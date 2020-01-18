// Resources.cs
// Copyright Karel Kroeze, 2017-2017

using System.Xml;
using UnityEngine;
using Verse;

namespace AnimalTab
{
    [StaticConstructorOnStartup]
    public static class Resources
    {
        public static Texture2D Background_Dark,
                                Background_Danger,
                                CheckOnWhite,
                                Filter,
                                Pregnant,
                                Egg,
                                Handling;

        public static Color Dark, Danger;

        static Resources()
        {
            Dark = new Color( 0f, 0f, 0f, .2f );
            Danger = new Color( 0.8f, 0f, 0f, .2f );

            Background_Dark = SolidColorMaterials.NewSolidColorTexture( Dark );
            Background_Danger = SolidColorMaterials.NewSolidColorTexture( Danger );
            CheckOnWhite = ContentFinder<Texture2D>.Get("UI/Icons/Animal/CheckOnWhite");
            Filter = ContentFinder<Texture2D>.Get( "UI/Icons/Filters/Filter" );
            Pregnant = ContentFinder<Texture2D>.Get("UI/Icons/Filters/Pregnant");
            Handling = ContentFinder<Texture2D>.Get( "UI/Icons/Filters/Training" );
            Egg = ContentFinder<Texture2D>.Get( "UI/Icons/Animal/Egg" );
        }
    }
}