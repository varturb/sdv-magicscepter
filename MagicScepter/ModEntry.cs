using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Tools;

namespace MagicScepter
{
    public class ModEntry : Mod
    {
        private List<Building> buildings;
        private List<MiniObeliskObject> miniObelisks;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var player = Game1.player;

            if (Context.IsWorldReady
                && Game1.activeClickableMenu == null
                && player.IsLocalPlayer
                && player.CurrentTool != null
                && player.CurrentTool is Wand
                && !player.isRidingHorse()
                && !player.bathingClothes
                && e.Button.IsUseToolButton())
            {
                ChooseWarpLocation();
            }
        }

        private void ChooseWarpLocation()
        {
            buildings = new List<Building>();
            miniObelisks = new List<MiniObeliskObject>();
            var responses = new List<Response>();
            responses.Add(new Response(WarpLocationChoice.Farm.ToString(), "Farm"));

            //var index = 1;
            //foreach (var obj in Game1.getFarm().objects.Pairs)
            //{
            //    if (obj.Value.Name == "Mini-Obelisk")
            //    {
            //        var name = $"{obj.Value.Name} #{index++}";
            //        var x = (int)obj.Value.tileLocation.X;
            //        var y = (int)obj.Value.tileLocation.Y;
            //        miniObelisks.Add(new MiniObeliskObject(name, x, y));
            //    }
            //}

            //if (miniObelisks.Count > 0)
            //    responses.Add(new Response("Mini-Obelisk", "Mini-Obelisk"));

            foreach (var building in Game1.getFarm().buildings)
            {
                switch (building.buildingType)
                {
                    case "Water Obelisk":
                        responses.Add(new Response(WarpLocationChoice.Beach.ToString(), "Beach"));
                        AddBuilding(building);
                        break;
                    case "Earth Obelisk":
                        responses.Add(new Response(WarpLocationChoice.Mountain.ToString(), "Mountain"));
                        AddBuilding(building);
                        break;
                    case "Desert Obelisk":
                        responses.Add(new Response(WarpLocationChoice.Desert.ToString(), "Desert"));
                        AddBuilding(building);
                        break;
                    case "Island Obelisk":
                        responses.Add(new Response(WarpLocationChoice.Island.ToString(), "Ginger Island"));
                        AddBuilding(building);

                        var islandWest = Game1.locations.First(loc => loc.name == "IslandWest") as IslandWest;
                        if (islandWest?.farmObelisk)
                            responses.Add(new Response(WarpLocationChoice.IslandFarm.ToString(), "Ginger Island Farm"));
                        break;
                    case "Woods Obelisk":
                        responses.Add(new Response(WarpLocationChoice.DeepWoods.ToString(), "Deep Woods"));
                        AddBuilding(building);
                        break;
                }
            }

            responses.Add(new Response(WarpLocationChoice.None.ToString(), "Cancel"));

            if (responses.Count == 2)
            {
                var to = WarpLocations.GetWarpLocation(WarpLocationChoice.Farm);
                WarpPlayerTo(to);
                return;
            }

            Game1.currentLocation.createQuestionDialogue(
                "Choose location:",
                responses.ToArray(),
                LocationAnswer);
        }

        private void LocationAnswer(Farmer farmer, string answer)
        {
            //if (answer == "Mini-Obelisk")
            //{
            //    var responses = miniObelisks.Select(mo => new Response(mo.Name, mo.Name));
            //    DelayedAction.functionAfterDelay(() =>
            //        Game1.currentLocation.createQuestionDialogue("Choose Mini-Obelisk:", responses.ToArray(), MiniObeliskAnswer), 100);
            //    return;
            //}

            Enum.TryParse(answer, out WarpLocationChoice choice);

            if (choice == WarpLocationChoice.None)
                return;

            if (choice == WarpLocationChoice.Farm ||
                choice == WarpLocationChoice.IslandFarm)
            {
                var wlocation = WarpLocations.GetWarpLocation(choice);
                WarpPlayerTo(wlocation);
                return;
            }

            var building = buildings.FirstOrDefault(b => GetWarpLocationChoiceForBuildingType(b.buildingType) == choice);
            if (building != null)
                building.doAction(new Vector2(building.tileX, building.tileY), farmer);
        }

        private void MiniObeliskAnswer(Farmer farmer, string answer)
        {
            var miniObelisk = miniObelisks.First(o => o.Name.Equals(answer));
            var warpLocationCoords = GetValidWarpTile(Game1.getFarm(), miniObelisk.CoordX, miniObelisk.CoordY);
            if (warpLocationCoords == null)
            {
                Game1.showRedMessage("Invalid Mini-Obelisk Warp Target Location");
                return;
            }
            var warpLocation = new WarpLocation("Farm", (int)warpLocationCoords.Value.X, (int)warpLocationCoords.Value.Y);
            WarpPlayerTo(warpLocation);
        }

        private void WarpPlayerTo(WarpLocation warpLocation)
        {
            DoBeforeWarpAnimation();
            DelayedAction.fadeAfterDelay(() => DoWarp(warpLocation), 1000);
            DoAfterWarpAnimation();
        }


        private void DoWarp(WarpLocation warpLocation)
        {
            Game1.warpFarmer(warpLocation.Name, warpLocation.CoordX, warpLocation.CoordY, false);

            if (!Game1.isStartingToGetDarkOut())
                Game1.playMorningSong();
            else
                Game1.changeMusicTrack("none");

            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.player.temporarilyInvincible = false;
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
        }

        private void DoBeforeWarpAnimation()
        {
            for (int index = 0; index < 12; ++index)
                Game1.player.currentLocation.temporarySprites.Add(
                    new TemporaryAnimatedSprite(
                        354,
                        Game1.random.Next(25, 75),
                        6,
                        1,
                        new Vector2(Game1.random.Next((int)Game1.player.position.X - Game1.tileSize * 4, (int)Game1.player.position.X + Game1.tileSize * 3),
                        Game1.random.Next((int)Game1.player.position.Y - Game1.tileSize * 4, (int)Game1.player.position.Y + Game1.tileSize * 3)),
                        false,
                        Game1.random.NextDouble() < 0.5
                     ));

            Game1.playSound("wand");
            Game1.displayFarmer = false;
            Game1.player.temporarilyInvincible = true;
            Game1.player.temporaryInvincibilityTimer = -2000;
            Game1.player.Halt();
            Game1.player.faceDirection(2);
            Game1.player.freezePause = 1000;
            Game1.flashAlpha = 1f;
        }

        private void DoAfterWarpAnimation()
        {
            new Rectangle(Game1.player.GetBoundingBox().X, Game1.player.GetBoundingBox().Y, Game1.tileSize, Game1.tileSize).Inflate(Game1.tileSize * 3, Game1.tileSize * 3);
            int num1 = 0;
            for (int index = Game1.player.getTileX() + 8; index >= Game1.player.getTileX() - 8; --index)
            {
                var temporarySprites = Game1.player.currentLocation.temporarySprites;
                var temporaryAnimatedSprite = new TemporaryAnimatedSprite(6, new Vector2(index, Game1.player.getTileY()) * Game1.tileSize, Color.White, 8, false, 50f, 0, -1, -1f, -1, 0);
                temporaryAnimatedSprite.layerDepth = 1f;
                int num2 = num1 * 25;
                temporaryAnimatedSprite.delayBeforeAnimationStart = num2;
                var vector2 = new Vector2(-0.25f, 0.0f);
                temporaryAnimatedSprite.motion = vector2;
                temporarySprites.Add(temporaryAnimatedSprite);
                ++num1;
            }
        }

        private void AddBuilding(Building building)
        {
            var existingBuilding = buildings.FirstOrDefault(b => b.buildingType == building.buildingType);
            if (existingBuilding == null)
            {
                buildings.Add(building);
            }
        }

        private WarpLocationChoice GetWarpLocationChoiceForBuildingType(string buildingType)
        {
            switch (buildingType)
            {
                case "Water Obelisk":
                    return WarpLocationChoice.Beach;
                case "Earth Obelisk":
                    return WarpLocationChoice.Mountain;
                case "Desert Obelisk":
                    return WarpLocationChoice.Desert;
                case "Island Obelisk":
                    return WarpLocationChoice.Island;
                case "Woods Obelisk":
                    return WarpLocationChoice.DeepWoods;
                default:
                    return WarpLocationChoice.None;
            }
        }

        private Vector2? GetValidWarpTile(GameLocation location, int x, int y)
        {
            var targetLocation = new Vector2(x, y + 1);
            if (location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(targetLocation))
            {
                return targetLocation;
            }
            targetLocation = new Vector2(x - 1, y);
            if (location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(targetLocation))
            {
                return targetLocation;
            }
            targetLocation = new Vector2(x + 1, y);
            if (location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(targetLocation))
            {
                return targetLocation;
            }
            targetLocation = new Vector2(x, y - 1);
            if (location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(targetLocation))
            {
                return targetLocation;
            }

            return null;
        }
    }
}
