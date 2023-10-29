using EmoMount;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;
using NodeCanvas.Tasks.Conditions;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EmoMount.EmoMountMod;

namespace EmoMount
{
    public class MountNPCManager
    {

        public List<StableMaster> StableMasters = new List<StableMaster>()
        {
            new StableMaster()
            {
                UID = "emomount.mountcharactercierzo",
                Name = "Emo, Cierzo Stable Master",
                SpawnSceneBuildName = "CierzoNewTerrain",
                SpawnPosition = new(1421.29f, 5.5604f, 1686.195f),
                SpawnRotation = new(0, 270f, 0),
                HelmetID = 3100091,
                ChestID = 3100090,
                BootsID = 3100092,
                WeaponID = 2100030,
                StartingPose = Character.SpellCastType.IdleAlternate,
                BuddySpecies = new List<string>()
                {
                    "PearlBird",
                    "SilverPearlBird",
                    "PearlBird",
                    "PearlBird"
                },
                //PearlBird Egg Basic
                BuyItemID = -26202,
                BuyText = $"I have a spare PearlBird egg for sale, 500 silver - have you ever tried training a PearlBird? That's a bargain.",
                BuyPrice = 500,
                HasUniqueBuyable = true,
                //vendavel succeded
                UniqueBuyQuestID = 7011004,
                //alpha coral horn
                UniqueBuyableID = -26302,
                UniqueBuyText =  $"I have some <color=green>Alpha CoralHorns</color> for sale since you helped with those bastards in Vendavel - for you 500 silver.",
                UniqueSellableHint = $"Sorry I cannot ensure I can feed my own <color=green>Alpha CoralHorns</color> I already have due to the problems we've been having with that lot of gits in Vendavel bloody Bandit 'Fortress'," +
                $" the last feed merchant through thought it was a bloody inn! Got robbed of everything.. if they were gone on the other hand..."
            },
            new StableMaster()
            {
                UID = "emomount.mountcharactermonsoon",
                Name = "Faeryn, Monsoon Stable Master",
                SpawnSceneBuildName = "Monsoon",
                SpawnPosition = new(82.0109f, -5.1698f, 140.1947f),
                SpawnRotation = new(0, 254.089f, 0),
                HelmetID = 3100091,
                ChestID = 3100090,
                BootsID = 3100092,
                WeaponID = 2100030,
                CharVisualData =
                {
                    Gender =  Character.Gender.Female
                },
                StartingPose = Character.SpellCastType.IdleAlternate,
                BuyItemID = -26310,
                BuyPrice = 500,
                BuyText = $"I can sell you a <color=red>Tuanosaur</color egg - due to hatch very soon! 500 silver, I am required by local by-law to state these little buggers nip.",
                HasUniqueBuyable = false
            },
            new StableMaster()
            {
                UID = "emomount.mountcharacterberg",
                Name = "Iggy the Wild, Berg Stable Master",
                SpawnSceneBuildName = "Berg",
                SpawnPosition = new(1191.945f, -13.7222f, 1383.581f),
                SpawnRotation = new(0, 72f, 0),
                HelmetID = 3100091,
                ChestID = 3100090,
                BootsID = 3100092,
                WeaponID = 2100030,
                StartingPose = Character.SpellCastType.IdleAlternate,
                BuyItemID = -26309,
                BuyPrice = 500,
                BuyText = $"I can sell you the whistle for this <color=green>Coral Horn Doe</color>, 500 silver.",
                //bring back a manticore tail quest event
                HasUniqueBuyable = true,
                UniqueBuyQuestID = 7011606,
                //JewelBird???
                UniqueBuyableID = -26314,
                UniqueBuyText =  $"Excellent thank you - Manticore venom has many uses. I can sell you this<color=purple> Jewel Bird</color> Whistle for 800 silver.",
                UniqueSellableHint = $"If you can help Chef Iasu with his Manticore 'problem'. I might have some nice <color=purple> Jewel Birds </color> I could sell you."

            },
            new StableMaster()
            {
                UID = "emomount.mountcharacterlevant",
                Name = "Ianis, Levant Stable Master",
                SpawnSceneBuildName = "Levant",
                SpawnPosition = new(-39.7222f, 0.2239f, 120.0354f),
                SpawnRotation = new(0, 218f, 0),
                HelmetID = 3100091,
                ChestID = 3100090,
                BootsID = 3100092,
                WeaponID = 2100030,
                StartingPose = Character.SpellCastType.IdleAlternate,
                BuyItemID = -26307,
                BuyPrice = 500,
                BuyText = "I can sell you the whistle for a <color=yellow>Crescent Shark</color>, great for desert travel, 500 silver!",
                HasUniqueBuyable = true,
                //bring back shark cartilage
                UniqueBuyQuestID = 7011613,
                UniqueBuyableID = -26315,
                UniqueSellableHint = $"Chef Tenno wants some shark fins bringing back, I need someone to thin them out a bit, speak with me after you've helped Chef Tenno out and I can sell you an <color=yellow>Elite Crescent Shark</color>.",
                UniqueBuyText =  $"Thanks for your help the deserts are a little safer for travelers thanks to you. I can sell you this particular variant of the <color=yellow>Elite Crescent Shark</color> for 800 silver."
            },
            new StableMaster()
            {
                UID = "emomount.mountcharacterharmattan",
                Name = "Libre, Harmattan Stable Master",
                SpawnSceneBuildName = "Harmattan",
                SpawnPosition = new(83.745f, 64.8413f, 802.2937f),
                SpawnRotation = new(0, 156.0281f, 0),
                HelmetID = 3100261,
                ChestID = 3100260,
                BootsID = 3100252,
                WeaponID = 2140080,
                StartingPose = Character.SpellCastType.IdleAlternate,
                CharVisualData = new SideLoader.SL_Character.VisualData()
                {
                    Gender = Character.Gender.Female,
                    SkinIndex = 2,
                    HairStyleIndex = 8,
                    HairColorIndex = 4,
                    HeadVariationIndex = 4
                }, BuddySpecies = new List<string>()
                {
                    "PearlBird"
                },
                BuyItemID = -26305,
                BuyPrice = 500,
                BuyText = "I can sell you the controller for a <color=cyan> Beast Golem </color> for 500 Silver.",
                HasUniqueBuyable = false
            },
            new StableMaster()
            {
                UID = "emomount.mountcharactercaldera",
                Name = "Schnabeldoktor, New Sirocco Stable Master",
                SpawnSceneBuildName = "NewSirocco",
                SpawnPosition = new(46.7443f, 55.8582f, -68.3737f),
                SpawnRotation = new(0, 295f, 0),
                HelmetID = 3000303,
                ChestID =  3000360,
                BootsID = 3100262,
                WeaponID = 2150051,
                StartingPose = Character.SpellCastType.IdleAlternate,
                CharVisualData = new SideLoader.SL_Character.VisualData()
                {
                    Gender = Character.Gender.Male,
                    SkinIndex = 2,
                    HairStyleIndex = 8,
                    HairColorIndex = 4,
                    HeadVariationIndex = 4
                },
                 
                BuyItemID = -26306,
                BuyPrice = 500,
                BuyText = "I can sell you the whistle for a <color=red> Fire Beetle</color>, for a mere 500 silver!",
                HasUniqueBuyable = false
            }

        };

        public MountNPCManager()
        {
            CreateNPCs();
        }

        private void CreateNPCs()
        {
            foreach (var StableMsater in StableMasters)
            {
                SetupNPC(StableMsater);
            }
        }

        private void SetupNPC(StableMaster StableMaster)
        {
            EmoMountMod.LogMessage($"EmoMountMod :: Setting up NPC {StableMaster.Name}");

            // Create and apply the template
            var template = StableMaster.CreateAndApplyTemplate((SL_Character SLTemplate, Character Character, string RPCData) =>
            {
                EmoMountMod.LogMessage($"EmoMountMod :: Applying Template to  {SLTemplate.Name}");

                GameObject dialogueTemplate = GameObject.Instantiate(Resources.Load<GameObject>("editor/templates/DialogueTemplate"));
                dialogueTemplate.transform.parent = Character.transform;
                dialogueTemplate.transform.position = Character.transform.position;

                // set Dialogue Actor name
                DialogueActor ourActor = Character.GetComponentInChildren<DialogueActor>();
                ourActor.SetName(SLTemplate.Name);

                // setup dialogue tree
                DialogueTreeController graphController = Character.GetComponentInChildren<DialogueTreeController>();
                Graph graph = graphController.graph;

                // the template comes with an empty ActorParameter, we can use that for our NPC actor.
                List<DialogueTree.ActorParameter> actors = (graph as DialogueTree).actorParameters;
                actors[0].actor = ourActor;
                actors[0].name = ourActor.name;


                StableMaster.BuildDialogueForCharacter((DialogueTree)graph, Character);
                CreateBuddySpeciesForStableMaster(StableMaster, Character);
            });

            template.ShouldSpawn = () => true;
        }

        private void CreateBuddySpeciesForStableMaster(StableMaster DialogueCharacter, Character Character)
        {
            if (DialogueCharacter.BuddySpecies != null && DialogueCharacter.BuddySpecies.Count > 0)
            {
                for (int i = 0; i < DialogueCharacter.BuddySpecies.Count; i++)
                {
                    if (!string.IsNullOrEmpty(DialogueCharacter.BuddySpecies[i]))
                    {
                      DialogueCharacter.SpawnCharacterMountBuddy(Character, DialogueCharacter.BuddySpecies[i], i, true);
                    }
                }
            }
        }
    }
}
