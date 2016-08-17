using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BetterAnimalsTab
{
    [StaticConstructorOnStartup]
    public static class Widgets_PetFollow
    {
        private const string PF_MOD_NAME = "PetFollow";
        private const string PF_TYPE_UTILS = "PetFollow.PetFollow_Utils";
        private const string PF_FIELD_DEFNAME_FOLLOW_HUNTER = "DESIGNATION_NAME_HUNT";
        private const string PF_FIELD_DEFNAME_FOLLOW_DRAFTED = "DESIGNATION_NAME_DRAFTED";
        private const string PF_METHOD_THING_FOLLOWABLE = "thingIsFollowable";
        private const string PF_METHOD_GET_FOLLOW_HUNTED = "hasHuntDesignation";
        private const string PF_METHOD_GET_FOLLOW_DRAFTED = "hasDraftedDesignation";
        private const string PF_METHOD_SET_FOLLOW = "setDesignation";
        private const BindingFlags BINDINGDLAGS_ALL = (BindingFlags) 60; // public + private + static + instance;

        public static Texture2D FollowHuntIcon;
        public static Texture2D FollowDraftIcon;

        private static bool _anyError;
        private static bool _initialized;
        private static bool _available;
        
        private static MethodInfo _hasDraftedDesignationMethodInfo;
        private static MethodInfo _hasHuntDesignationMethodInfo;
        private static MethodInfo _setDesignationMethodInfo;
        private static MethodInfo _thingIsFollowableMethodInfo;
        private static string _designationNameFollowHunter;
        private static string _designationNameFollowDrafted;

        public static bool PetFollowAvailable
        {
            get
            {
                if ( !_initialized )
                    Initialize();
                return ( _available && !_anyError );
            }
        }

        private static void Initialize()
        {
            _available = LoadedModManager.RunningMods.Any( mod => mod.Name == PF_MOD_NAME );
            _initialized = true;

            if ( _available )
            {
                try
                {
                    // get the assembly
                    var PF_assembly = LoadedModManager
                                        .RunningMods.First( mod => mod.Name == PF_MOD_NAME )
                                        .assemblies.loadedAssemblies.First();

                    if (PF_assembly == null )
                        throw new Exception( "PetFollow assembly not found." );

                    // get the Utils type
                    var PF_UtilsType = PF_assembly.GetType( PF_TYPE_UTILS );
                    if (PF_UtilsType == null )
                        throw new Exception( "PetFollow utilities type not found.");

                    // get the various fields and methods we need.
                    // methods: get drafted following
                    _hasDraftedDesignationMethodInfo = PF_UtilsType.GetMethod( PF_METHOD_GET_FOLLOW_DRAFTED,
                                                                               BINDINGDLAGS_ALL );
                    if ( _hasDraftedDesignationMethodInfo == null )
                        throw new Exception( "PetFollow method not found: " + PF_METHOD_GET_FOLLOW_DRAFTED );

                    // methods: get hunter following
                    _hasHuntDesignationMethodInfo = PF_UtilsType.GetMethod( PF_METHOD_GET_FOLLOW_HUNTED,
                                                                               BINDINGDLAGS_ALL );
                    if ( _hasHuntDesignationMethodInfo == null )
                        throw new Exception( "PetFollow method not found: " + PF_METHOD_GET_FOLLOW_HUNTED );

                    // methods: set following
                    _setDesignationMethodInfo = PF_UtilsType.GetMethod( PF_METHOD_SET_FOLLOW, BINDINGDLAGS_ALL );
                    if ( _setDesignationMethodInfo == null )
                        throw new Exception( "PetFollow method not found: " + PF_METHOD_SET_FOLLOW );

                    // methods: things is 'followable' (i.e. has master, obedience, player faction)
                    _thingIsFollowableMethodInfo = PF_UtilsType.GetMethod( PF_METHOD_THING_FOLLOWABLE, BINDINGDLAGS_ALL );
                    if ( _thingIsFollowableMethodInfo == null )
                        throw new Exception( "PetFollow method not found: " + PF_METHOD_THING_FOLLOWABLE );

                    // fields: hunter follow designation name
                    _designationNameFollowHunter =
                        PF_UtilsType.GetField( PF_FIELD_DEFNAME_FOLLOW_HUNTER, BINDINGDLAGS_ALL )?.GetValue( null ) as string;
                    if ( _designationNameFollowHunter.NullOrEmpty() )
                        throw new Exception( "PetFollow field not found: " + PF_FIELD_DEFNAME_FOLLOW_HUNTER );
                    
                    // fields: drafted follow designation name
                    _designationNameFollowDrafted =
                        PF_UtilsType.GetField( PF_FIELD_DEFNAME_FOLLOW_DRAFTED, BINDINGDLAGS_ALL )?.GetValue( null ) as string;
                    if ( _designationNameFollowDrafted.NullOrEmpty() )
                        throw new Exception( "PetFollow field not found: " + PF_FIELD_DEFNAME_FOLLOW_DRAFTED );

                    // icons: hunter
                    FollowHuntIcon = ContentFinder<Texture2D>.Get( "HuntFollow" );
                    if ( FollowHuntIcon == null )
                        throw new Exception( "PetFollow icon not found: HuntFollow" );

                    // icons: drafted
                    FollowDraftIcon = ContentFinder<Texture2D>.Get( "DraftFollow" );
                    if ( FollowDraftIcon == null )
                        throw new Exception( "PetFollow icon not found: DraftFollow" );

                    Log.Message( "Animal Tab :: PetFollow functionality integrated" );
                }
                catch
                {
                    _anyError = true;
                    Log.Error( "Animal Tab :: Error in PetFollow integration - functionality disabled" );
                    throw;
                }
            }
        }

        public static bool CanFollow( this Pawn animal )
        {
            return (bool) _thingIsFollowableMethodInfo.Invoke( null, new object[] {animal} );
        }

        public static bool FollowsDrafted( this Pawn animal )
        {
            return (bool)_hasDraftedDesignationMethodInfo.Invoke( null, new object[] { animal } );
        }

        public static void FollowsDrafted( this Pawn animal, bool set )
        {
            _setDesignationMethodInfo.Invoke( null, new object[] { animal, _designationNameFollowDrafted, set } );
        }
        public static bool FollowsHunter( this Pawn animal )
        {
            return (bool)_hasHuntDesignationMethodInfo.Invoke( null, new object[] { animal } );
        }

        public static void FollowsHunter( this Pawn animal, bool set )
        {
            _setDesignationMethodInfo.Invoke( null, new object[] { animal, _designationNameFollowHunter, set } );
        }

        public static void ToggleAllFollowsHunter( List<Pawn> animals )
        {
            int count = animals.Count();
            bool[] following = animals.Select( a => a.FollowsHunter() ).ToArray();
            bool[] canFollow = animals.Select( a => a.CanFollow() ).ToArray();
            bool anyCanFollow = false;
            bool anyFollowing = false;
            bool all = true;

            for ( int i = 0; i < count; i++ )
            {
                if ( !anyCanFollow && canFollow[i] )
                    anyCanFollow = true;
                if ( !anyFollowing && following[i] )
                    anyFollowing = true;
                if ( all && !following[i] )
                    all = false;
            }

            if ( anyCanFollow )
            {
                for ( int i = 0; i < count; i++ )
                {
                    if ( canFollow[i] && !all && !following[i] )
                        animals[i].FollowsHunter( true );
                    if ( canFollow[i] && all )
                        animals[i].FollowsHunter( false );
                }
                if ( all )
                    SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                else
                    SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
            }
        }

        public static void ToggleAllFollowsDrafted( List<Pawn> animals )
        {
            int count = animals.Count();
            bool[] following = animals.Select( a => a.FollowsDrafted() ).ToArray();
            bool[] canFollow = animals.Select( a => a.CanFollow() ).ToArray();
            bool anyCanFollow = false;
            bool anyFollowing = false;
            bool all = true;

            for ( int i = 0; i < count; i++ )
            {
                if ( !anyCanFollow && canFollow[i] )
                    anyCanFollow = true;
                if ( !anyFollowing && following[i] )
                    anyFollowing = true;
                if ( all && canFollow[i] && !following[i] )
                    all = false;
            }

            if ( anyCanFollow )
            {
                for ( int i = 0; i < count; i++ )
                {
                    if ( canFollow[i] && !all && !following[i] )
                        animals[i].FollowsDrafted( true );
                    if ( canFollow[i] && all )
                        animals[i].FollowsDrafted( false );
                }
                if ( all )
                    SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
                else
                    SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
            }
        }
    }
}
