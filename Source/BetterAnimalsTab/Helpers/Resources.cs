using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterAnimalsTab
{
    [StaticConstructorOnStartup]
    public static class Resources
    {
        #region Fields

        public const float rowHeight = 30f;
        public const float widthPerTrainable = 20f;
        public const float iconSize = 16f;

        public static readonly Texture2D BarBg = SolidColorMaterials.NewSolidColorTexture( 0f, 1f, 0f, 0.8f );

        public static readonly Texture2D CheckWhite = ContentFinder<Texture2D>.Get( "UI/Buttons/CheckOnWhite" );

        public static readonly Texture2D FilterOffTex = ContentFinder<Texture2D>.Get( "UI/Buttons/filter_off_large" );

        public static readonly Texture2D FilterTex = ContentFinder<Texture2D>.Get( "UI/Buttons/filter_large" );

        public static readonly Texture2D[] GenderTextures =
        {
            ContentFinder<Texture2D>.Get( "UI/Gender/none" ),
            ContentFinder<Texture2D>.Get( "UI/Gender/male" ),
            ContentFinder<Texture2D>.Get( "UI/Gender/female" )
        };

        public static readonly Texture2D[] LifeStageTextures =
        {
            ContentFinder<Texture2D>.Get( "UI/LifeStage/1" ),
            ContentFinder<Texture2D>.Get( "UI/LifeStage/2" ),
            ContentFinder<Texture2D>.Get( "UI/LifeStage/3" ),
            ContentFinder<Texture2D>.Get( "UI/LifeStage/unknown" )
        };

        public static readonly Texture2D PregnantTex = ContentFinder<Texture2D>.Get( "UI/FilterStates/pregnant_large" );

        public static readonly Texture2D SlaughterTex = ContentFinder<Texture2D>.Get( "UI/Buttons/slaughter" );

        private static Dictionary<TrainableDef, Texture2D> trainableTextures = new Dictionary<TrainableDef, Texture2D>();
        private static HashSet<TrainableDef> failedFindingTexture = new HashSet<TrainableDef>();

        public static Texture2D GetUIIcon( this TrainableDef trainable )
        {
            // did we fail finding this before?
            if ( failedFindingTexture.Contains( trainable ) )
                return null;

            // try get from cache
            Texture2D texture;
            if ( trainableTextures.TryGetValue( trainable, out texture ) )
                return texture;

            // first time trying to get this texture
#if DEBUG
            texture = ContentFinder<Texture2D>.Get( $"UI/Training/{trainable.defName}", true );
#else
            texture = ContentFinder<Texture2D>.Get($"UI/Training/{trainable.defName}", false);
#endif

            if ( texture == null )
            {
                Log.Warning( $"Failed to get UI Icon for {trainable.LabelCap} (defName: {trainable.defName}). UI Icon should be placed at '[YourMod]/Textures/UI/Training/{trainable.defName}'." );
                failedFindingTexture.Add( trainable );
                return null;
            }
            else
            {
                trainableTextures.Add( trainable, texture );
                return texture;
            }
        }

        public static readonly Texture2D WorkBoxCheckTex = ContentFinder<Texture2D>.Get( "UI/Widgets/WorkBoxCheck" );

#endregion Fields
    }
}
