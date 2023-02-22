using SideLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmoMount.Custom_SL_Effect
{
    public class SL_MountRandomEmission : SL_Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_MountRandomEmission);
        public Type GameModel => typeof(MountRandomEmission);

        public List<Color> ColorChoices;
        public float Intensity = 7f;
        public float ChangeInterval = 0.1f;

        public override void ApplyToComponent<T>(T component)
        {
            MountRandomEmission comp = component as MountRandomEmission;
            comp.ColorChoices = ColorChoices.ToList();
            comp.Intensity = Intensity;
            comp.ChangeInterval = ChangeInterval;
        }

        public override void SerializeEffect<T>(T effect)
        {

        }
    }

    public class MountRandomEmission : Effect
    {
        public List<Color> ColorChoices;
        public float Intensity = 7f;
        public float ChangeInterval = 0.1f;
        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            CharacterMount characterMount = _affectedCharacter.gameObject.GetComponent<CharacterMount>();

            if (characterMount != null && characterMount.HasActiveMount)
            {
                if (characterMount.ActiveMountDisabled)
                {
                    characterMount.EnableActiveMount();
                }

                _affectedCharacter.StartCoroutine(ColorSlots(characterMount.ActiveMount));
            }
        }


        private IEnumerator ColorSlots(BasicMountController mountController)
        {
            List<Color> CopyOfColours = ColorChoices.ToList();

            Color Chosen = GetAndRemoveRandom(CopyOfColours);
            CopyOfColours.OrderBy(x => new Guid());
            mountController.SetEmissionColor(GetAndRemoveRandom(CopyOfColours));
            float Timer = 0;

            while (CopyOfColours.Count > 0)
            {
                Timer += Time.deltaTime;

                if (Timer >= ChangeInterval)
                {
                    mountController.SetEmissionColor(GetAndRemoveRandom(CopyOfColours), Intensity);
                    Timer = 0;

                }

                yield return null;
            }
            mountController.SetEmissionColor(Chosen, Intensity);
            yield break;
        }


        private Color GetAndRemoveRandom(List<Color> ColorChoices)
        {
            int RandIndex = UnityEngine.Random.Range(0, ColorChoices.Count);
            Color Chosen = ColorChoices[RandIndex];
            ColorChoices.RemoveAt(RandIndex);
            return Chosen;
        }
    }
}
