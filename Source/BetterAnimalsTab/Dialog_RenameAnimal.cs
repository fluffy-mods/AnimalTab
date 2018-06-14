// Dialog_RenameAnimal.cs
// Copyright Karel Kroeze, 2017-2017

using RimWorld;
using Verse;

namespace AnimalTab
{
    public class Dialog_RenameAnimal : Dialog_Rename
    {
        private Pawn animal;
        private string oldName;

        public Dialog_RenameAnimal( Pawn animal )
        {
            this.animal = animal;
            curName = animal.Name.ToString();
            oldName = curName;
        }

        protected override void SetName( string name )
        {
            animal.Name = new NameSingle( curName );
            Messages.Message( "AnimalTab.AnimalRenamed".Translate( oldName, curName ), MessageTypeDefOf.SilentInput );
        }
    }
}