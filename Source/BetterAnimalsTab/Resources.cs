// Resources.cs
// Copyright Karel Kroeze, 2017-2017

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
            Pregnant;

        static Resources()
        {
            CheckboxBackground = SolidColorMaterials.NewSolidColorTexture(0f, 0f, 0f, .2f);
            CheckboxBackground_Danger = SolidColorMaterials.NewSolidColorTexture(0.8f, 0f, 0f, .2f);
            CheckOnWhite = ContentFinder<Texture2D>.Get("UI/Icons/Animal/CheckOnWhite");
            Filter = ContentFinder<Texture2D>.Get( "UI/Icons/Filters/Filter" );
            Pregnant = ContentFinder<Texture2D>.Get("UI/Icons/Filters/Pregnant");
    }
    }
}