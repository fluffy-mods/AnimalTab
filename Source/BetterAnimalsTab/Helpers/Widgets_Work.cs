//// // Karel Kroeze
//// // Widgets_Work.cs
//// // 2016-06-27

//using System.Globalization;
//using System.Text;
//using RimWorld;
//using UnityEngine;
//using Verse;
//using Verse.Sound;

//namespace Fluffy
//{
//    public static class Widgets_Work
//    {
//        private const float WorkBoxSize = 25f;
//        private const int MidAptCutoff = 14;
//        private const float PassionOpacity = 0.4f;
//        private const int ClickThreshold = 25;
//        public static bool PrioritiesDirty = false;
//        private static Pair<Pawn, WorkTypeDef> _mouseDownOn;
//        private static int _priority = -1;
//        private static int _clickLenth;

//        private static readonly Texture2D WorkBoxBGTex_Bad = ContentFinder<Texture2D>.Get( "UI/Widgets/WorkBoxBG_Bad" );
//        private static readonly Texture2D WorkBoxBGTex_Mid = ContentFinder<Texture2D>.Get( "UI/Widgets/WorkBoxBG_Mid" );

//        private static readonly Texture2D WorkBoxBGTex_Excellent =
//            ContentFinder<Texture2D>.Get( "UI/Widgets/WorkBoxBG_Excellent" );

//        private static readonly Texture2D WorkBoxCheckTex = ContentFinder<Texture2D>.Get( "UI/Widgets/WorkBoxCheck" );

//        private static readonly Texture2D PassionWorkboxMinorIcon =
//            ContentFinder<Texture2D>.Get( "UI/Icons/PassionMinorGray" );

//        private static readonly Texture2D PassionWorkboxMajorIcon =
//            ContentFinder<Texture2D>.Get( "UI/Icons/PassionMajorGray" );

//        private static Color ColorOfPriority( int prio )
//        {
//            switch ( prio )
//            {
//                case 1:
//                    return Color.green;
//                case 2:
//                    return new Color( 1f, 0.9f, 0.6f );
//                case 3:
//                    return new Color( 0.8f, 0.7f, 0.5f );
//                case 4:
//                    return new Color( 0.6f, 0.6f, 0.6f );
//                default:
//                    return Color.grey;
//            }
//        }

//        public static void DrawWorkBoxFor( Vector2 topLeft, Pawn p, WorkTypeDef wType )
//        {
//            if ( p.story == null || p.story.WorkTypeIsDisabled( wType ) )
//            {
//                return;
//            }
//            var rect = new Rect( topLeft.x, topLeft.y, WorkBoxSize, WorkBoxSize );
//            DrawWorkBoxBackground( rect, p, wType );
//            if ( Find.PlaySettings.useWorkPriorities )
//            {
//                int priority = p.workSettings.GetPriority( wType );
//                string label = priority > 0 ? priority.ToString() : string.Empty;
//                Text.Anchor = TextAnchor.MiddleCenter;
//                GUI.color = ColorOfPriority( priority );
//                Rect rect2 = rect.ContractedBy( -3f );
//                Widgets.Label( rect2, label );
//                GUI.color = Color.white;
//                Text.Anchor = TextAnchor.UpperLeft;
//                if ( Event.current.type == EventType.MouseDown && Mouse.IsOver( rect ) )
//                {
//                    if ( Event.current.button == 0 )
//                    {
//                        int num = p.workSettings.GetPriority( wType ) - 1;
//                        if ( num < 0 )
//                        {
//                            num = 4;
//                        }
//                        p.workSettings.SetPriority( wType, num );
//                        SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
//                    }
//                    if ( Event.current.button == 1 )
//                    {
//                        int num2 = p.workSettings.GetPriority( wType ) + 1;
//                        if ( num2 > 4 )
//                        {
//                            num2 = 0;
//                        }
//                        p.workSettings.SetPriority( wType, num2 );
//                        SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
//                    }
//                    Event.current.Use();
//                }
//            }
//            else
//            {
//                int priority2 = p.workSettings.GetPriority( wType );
//                if ( priority2 > 0 )
//                {
//                    GUI.DrawTexture( rect, WorkBoxCheckTex );
//                }
//                if ( Mouse.IsOver( rect ) )
//                {
//                    // catch clicks
//                    // for some reason down & up get called 4 times, make sure 
//                    if ( Input.GetMouseButtonDown( 0 ) && _mouseDownOn.First == null )
//                    {
//                        _mouseDownOn = new Pair<Pawn, WorkTypeDef>( p, wType );
//                        _priority = p.workSettings.GetPriority( wType );
//                        _clickLenth = 0;
//                    }
//                    if ( Input.GetMouseButtonUp( 0 ) )
//                    {
//                        if ( p == _mouseDownOn.First && wType == _mouseDownOn.Second )
//                        {
//                            if ( _priority < 1 )
//                            {
//                                p.workSettings.SetPriority( wType, 3 );
//                                SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
//                            }
//                            else
//                            {
//                                p.workSettings.SetPriority( wType, 0 );
//                                SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
//                            }
//                        }
//                        // for some reason mouseDown & mouseUp get registered 4 times
//                        // set the tracker to null to avoid further down calls and immediate resets
//                        _mouseDownOn = new Pair<Pawn, WorkTypeDef>( null, null );
//                        _priority = -1;
//                        _clickLenth = 0;
//                    }
//                    // catch drags
//                    if ( Input.GetMouseButton( 0 ) )
//                    {
//                        // if this is the cell that a click originated in, delay dragging action until threshold is reached.
//                        if ( p == _mouseDownOn.First && wType == _mouseDownOn.Second )
//                        {
//                            if ( _clickLenth++ < ClickThreshold )
//                            {
//                                return;
//                            }
//                        }

//                        // Log.Message(p.workSettings.GetPriority(wType).ToString());
//                        // Priority of 'active' is 1 when manual is disabled, even if set to 3
//                        if ( p.workSettings.GetPriority( wType ) < 1 )
//                        {
//                            p.workSettings.SetPriority( wType, 3 );
//                            SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera();
//                        }
//                    }
//                    else if ( Input.GetMouseButton( 1 ) )
//                    {
//                        if ( p.workSettings.GetPriority( wType ) > 0 )
//                        {
//                            p.workSettings.SetPriority( wType, 0 );
//                            SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera();
//                        }
//                    }
//                }
//            }
//        }

//        public static TipSignal TipForPawnWorker( Pawn p, WorkTypeDef wDef )
//        {
//            var stringBuilder = new StringBuilder();
//            stringBuilder.AppendLine( wDef.gerundLabel );
//            if ( p.story.WorkTypeIsDisabled( wDef ) )
//            {
//                stringBuilder.Append( "CannotDoThisWork".Translate( p.NameStringShort ) );
//            }
//            else
//            {
//                string text = string.Empty;
//                if ( wDef.relevantSkills.Count == 0 )
//                {
//                    text = "NoneBrackets".Translate();
//                }
//                else
//                {
//                    foreach ( SkillDef current in wDef.relevantSkills )
//                    {
//                        text = text + current.skillLabel + ", ";
//                    }
//                    text = text.Substring( 0, text.Length - 2 );
//                }
//                stringBuilder.AppendLine( "RelevantSkills".Translate( text,
//                                                                      p.skills.AverageOfRelevantSkillsFor( wDef )
//                                                                       .ToString( CultureInfo.InvariantCulture ), 20 ) );
//                stringBuilder.AppendLine();
//                stringBuilder.Append( wDef.description );
//            }
//            return stringBuilder.ToString();
//        }

//        private static void DrawWorkBoxBackground( Rect rect, Pawn p, WorkTypeDef workDef )
//        {
//            float num = p.skills.AverageOfRelevantSkillsFor( workDef );
//            Texture2D image;
//            Texture2D image2;
//            float a;
//            if ( num <= MidAptCutoff )
//            {
//                image = WorkBoxBGTex_Bad;
//                image2 = WorkBoxBGTex_Mid;
//                a = num / MidAptCutoff / 1f;
//            }
//            else
//            {
//                image = WorkBoxBGTex_Mid;
//                image2 = WorkBoxBGTex_Excellent;
//                a = ( num - MidAptCutoff ) / 6f;
//            }
//            GUI.DrawTexture( rect, image );
//            GUI.color = new Color( 1f, 1f, 1f, a );
//            GUI.DrawTexture( rect, image2 );
//            Passion passion = p.skills.MaxPassionOfRelevantSkillsFor( workDef );
//            if ( passion > Passion.None )
//            {
//                GUI.color = new Color( 1f, 1f, 1f, PassionOpacity );
//                Rect position = rect;
//                position.xMin = rect.center.x;
//                position.yMin = rect.center.y;
//                if ( passion == Passion.Minor )
//                {
//                    GUI.DrawTexture( position, PassionWorkboxMinorIcon );
//                }
//                else if ( passion == Passion.Major )
//                {
//                    GUI.DrawTexture( position, PassionWorkboxMajorIcon );
//                }
//            }
//            GUI.color = Color.white;
//        }
//    }
//}
