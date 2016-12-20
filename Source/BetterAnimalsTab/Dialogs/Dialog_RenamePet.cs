// // Karel Kroeze
// // Dialog_RenamePet.cs
// // 2016-06-27

using UnityEngine;
using Verse;

namespace Fluffy
{
    public class Dialog_RenamePet : Window
    {
        #region Fields

        private readonly Pawn _pet;
        private string _curName;

        #endregion Fields

        #region Constructors

        public Dialog_RenamePet( Pawn pet )
        {
            _pet = pet;
            _curName = pet.Name.ToString();
            closeOnEscapeKey = true;
            absorbInputAroundWindow = true;
        }

        #endregion Constructors

        #region Properties

        public override Vector2 InitialSize => new Vector2( 500f, 200f );

        #endregion Properties

        #region Methods

        public override void DoWindowContents( Rect inRect )
        {
            Text.Font = GameFont.Small;
            var flag = false;
            if ( Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return )
            {
                flag = true;
                Event.current.Use();
            }
            Widgets.Label( new Rect( 0f, 0f, inRect.width, inRect.height ), "Fluffy.PetName".Translate() );
            _curName = Widgets.TextField( new Rect( 0f, inRect.height - 35f, inRect.width / 2f - 20f, 35f ), _curName );
            if (
                Widgets.ButtonText(
                                   new Rect( inRect.width / 2f + 20f, inRect.height - 35f, inRect.width / 2f - 20f, 35f ),
                                   "OK".Translate() ) || flag )
            {
                if ( IsValidName( _curName ) )
                {
                    _pet.Name = new NameSingle( _curName );
                    Find.WindowStack.TryRemove( this );
                    Messages.Message( "Fluffy.PetRenamed".Translate(), MessageSound.Benefit );
                }
                else
                {
                    Messages.Message( "Fluffy.PetInvalidName".Translate(), MessageSound.RejectInput );
                }
                Event.current.Use();
            }
        }

        private bool IsValidName( string s )
        {
            return s.Length != 0 && GenText.IsValidFilename( s );
        }

        #endregion Methods
    }
}
