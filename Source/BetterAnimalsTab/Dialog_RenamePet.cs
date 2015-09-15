using System;
using UnityEngine;
using Verse;

using RimWorld;

namespace Fluffy
{
    public class Dialog_RenamePet : Window
    {
        private Pawn pet;

        private string curName;

        public override Vector2 InitialWindowSize
        {
            get
            {
                return new Vector2(500f, 200f);
            }
        }

        public Dialog_RenamePet(Pawn pet)
        {
            this.pet = pet;
            this.curName = pet.Name.ToString();
            this.closeOnEscapeKey = true;
            this.absorbInputAroundWindow = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            bool flag = false;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                flag = true;
                Event.current.Use();
            }
            Widgets.Label(new Rect(0f, 0f, inRect.width, inRect.height), "Fluffy.PetName".Translate());
            this.curName = Widgets.TextField(new Rect(0f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), this.curName);
            if (Widgets.TextButton(new Rect(inRect.width / 2f + 20f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), "OK".Translate()) || flag)
            {
                if (this.IsValidName(this.curName))
                {
                    pet.Name = new NameSingle(this.curName);
                    Find.WindowStack.TryRemove(this);
                    Messages.Message("Fluffy.PetRenamed".Translate(), MessageSound.Benefit);
                }
                else
                {
                    Messages.Message("Fluffy.PetInvalidName".Translate(), MessageSound.RejectInput);
                }
                Event.current.Use();
            }
        }

        private bool IsValidName(string s)
        {
            return s.Length != 0 && GenText.IsValidFilename(s);
        }
    }
}