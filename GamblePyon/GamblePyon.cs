﻿using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace GamblePyon {
    public sealed class GamblePyon : IDalamudPlugin {
        public string Name => "GamblePyon";
        private const string CommandName = "/pyon";

        [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
        [PluginService] public static IChatGui ChatGui { get; private set; } = null!;
        [PluginService] public static IClientState ClientState { get; private set; } = null!;
        [PluginService] public static ISigScanner SigScanner { get; private set; } = null!;
        [PluginService] public static IPartyList PartyList { get; private set; } = null!;

        private WindowSystem Windows;
        private static MainWindow MainWindow;

        public static Chat Chat;

        public GamblePyon() {
            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
                HelpMessage = "Open Blackjack Interface."
            });

            PluginInterface.UiBuilder.OpenConfigUi += () => {
                MainWindow.IsOpen = true;
            };

            Chat = new Chat(SigScanner);
            Windows = new WindowSystem(Name);
            MainWindow = new MainWindow(this) { IsOpen = false };
            MainWindow.Config = PluginInterface.GetPluginConfig() as Config ?? new Config();
            MainWindow.Config.Initialize(PluginInterface);
            Windows.AddWindow(MainWindow);
            MainWindow.Initialize();

            PluginInterface.UiBuilder.Draw += Windows.Draw;
            ChatGui.ChatMessage += MainWindow.OnChatMessage;
        }

        public void Dispose() {
            PluginInterface.UiBuilder.Draw -= Windows.Draw;
            ChatGui.ChatMessage -= MainWindow.OnChatMessage;
            MainWindow.Dispose();
            CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args) {
            MainWindow.IsOpen = true;
        }
    }
}
