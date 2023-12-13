using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TweaksAndFixes.Scripts
{
    internal class GPQuitNoSaveButton : GoPointerButton
    {
        public override void OnActivate()
        {
            Application.Quit();
        }

        public GPQuitNoSaveButton()
        {
        }

        public StartMenu startMenu;
    }
}
