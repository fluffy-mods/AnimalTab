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
        public static Texture2D CheckboxBackground,
                                CheckboxBackground_Danger,
                                CheckOnWhite,
                                Filter,
                                Pregnant,
                                Handling;

        static Resources()
        {
            CheckboxBackground = SolidColorMaterials.NewSolidColorTexture(0f, 0f, 0f, .2f);
            CheckboxBackground_Danger = SolidColorMaterials.NewSolidColorTexture(0.8f, 0f, 0f, .2f);
            CheckOnWhite = ContentFinder<Texture2D>.Get("UI/Icons/Animal/CheckOnWhite");
            Filter = ContentFinder<Texture2D>.Get( "UI/Icons/Filters/Filter" );
            Pregnant = ContentFinder<Texture2D>.Get("UI/Icons/Filters/Pregnant");
            Handling = ContentFinder<Texture2D>.Get( "UI/Icons/Filters/Training" );
        }
    }
}