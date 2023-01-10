﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Input;
using TucanEngine.AssimpImplementation;
using TucanEngine.Display;
using TucanEngine.Gui;
using TucanEngine.Main;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.BasicComponents;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Physics;
using TucanEngine.Physics.Components;
using TucanEngine.Rendering;
using TucanEngine.Rendering.Components;
using TucanEngine.Scene;

namespace TucanPlus
{
    internal class Program
    {
        private const string TemporaryTag = "Temp";
        private const int PlatformCount = 10;
        
        private static List<GameObject> platforms = new List<GameObject>();

        public static void Main(string[] args) {
            var display = new Display(800, 600, "Tucan Display", () => {
                var guiSkin = new GuiSkin();
                guiSkin.SetFont(new Texture2D("resources\\font.png"));
                var guiManager = new GuiManager(guiSkin, new GuiShader());
                var scene = new Scene();
                var camera = scene.GetCamera();

                var player = new GameObject();
                player.LocalSpaceScale *= 1.8f;

                Physics.Gravity = -10;
                var text = guiManager.Text(string.Empty);
                text.WorldSpaceScale = new Vector3(1f, 0.4f, 1);
                text.WorldSpaceLocation = new Vector3(-0.5f, 0.4f, 0);
                scene.PushPool("Player", player, 1);

                var meshRenderer = new MeshRenderer();
                meshRenderer.SetMesh(ModelLoader.LoadMeshFromFile("grassPlatform.obj"));
                meshRenderer.SetTexture(new Texture2D("grassPlatform.png"));

                var model = new GameObject();
                model.AddBehaviour(meshRenderer);
                model.AddBehaviour<BoxComponent>();
                var platformCollision = model.GetBehaviour<BoxComponent>();
                platformCollision.IgnoreMtd = true;

                scene.PushPool(TemporaryTag, model, 10);
                scene.FillPools();
                var playerInstance =
                    scene.InstantiateFromPool("Player", Vector3.UnitY, Quaternion.Identity, new Vector3(1, 1.8f, 1));
                camera.SetParent(playerInstance);
                camera.LocalSpaceLocation = Vector3.UnitY * 0.5f;
                
                playerInstance.AddBehaviour<BoxComponent>();
                var box = playerInstance.GetBehaviour<BoxComponent>();
                box.CollisionEnter = (transform, direction) => {
                    if (direction is Face.Up && transform == platforms.Last()) {
                        text.SetText("You Win!");
                    }
                    playerInstance.SetParent(transform);
                };
                box.CollisionExit = (transform, direction) => {
                    playerInstance.SetParent(transform);
                };
                playerInstance.AddBehaviour<BasicFirstPersonController>();
                playerInstance.AddBehaviour<GameController>();

                var ran = new Random();
                for (var i = 0; i < PlatformCount; i++) {
                    var instance = scene.InstantiateFromPool(TemporaryTag,
                        (ran.Next(0, 10) * 0.1f - 3) * Vector3.UnitY
                        + Vector3.UnitZ * i * 8
                        + Vector3.UnitX * ran.Next(-3, 3) * Math.Sign(i), 
                        Quaternion.FromEulerAngles(0, MathHelper.DegreesToRadians(ran.Next(0,180)), 0), Vector3.One);
                    platforms.Add(instance);
                }
            });
        }

        public class GameController : Behaviour
        {
            private GameObject gameObject;
            private BoxComponent boxComponent;

            public override void OnLoad(EventArgs e) {
                base.OnLoad(e);
                gameObject = GetAssignedObject();
                boxComponent = gameObject.GetBehaviour<BoxComponent>();
                gameObject.WorldSpaceLocation = platforms[0].WorldSpaceLocation + Vector3.UnitY;
            }

            public override void OnUpdateFrame(FrameEventArgs e) {
                base.OnUpdateFrame(e);
                if (gameObject.WorldSpaceLocation.Y < -12) {
                    gameObject.LocalSpaceLocation = Vector3.UnitY;
                }

                foreach (var platform in platforms) {
                    platform.Rotate((float)e.Time, Vector3.UnitY);
                }

                if (Input.IsMouseButtonDown(MouseButton.Left)) {
                    if (Physics.Raycast(gameObject.WorldSpaceLocation, gameObject.Forward(Space.Global), out var hitInfo,
                        new IShape[] { boxComponent.GetBoxShape() })) {
                        ((GameObject)hitInfo.Item2?.AssignedTransform)?.SetActive(false);
                    }
                }
            }
        }
    }
}
