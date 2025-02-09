// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Community.PowerToys.Run.Plugin.ChatGPT.Services;
using ManagedCommon;
using Wox.Infrastructure;
using Wox.Plugin;
using Wox.Plugin.Logger;
using BrowserInfo = Wox.Plugin.Common.DefaultBrowserInfo;

namespace Community.PowerToys.Run.Plugin.ChatGPT
{
    public class Main : IPlugin, IPluginI18n, IContextMenu, IReloadable, IDisposable
    {
        // Should only be set in Init()
        private Action onPluginError;

        private PluginInitContext _context;

        private string _iconPath;

        private bool _disposed;

        private readonly ChatGPTService _chatGPTService = new ChatGPTService();

        public string Description => Properties.Resources.plugin_description;

        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            return new List<ContextMenuResult>(0);
        }

        public List<Result> Query(Query query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var results = new List<Result>();

            // empty query
            if (string.IsNullOrEmpty(query.Search))
            {
                string arguments = "https://chat.openai.com/";
                results.Add(new Result
                {
                    Title = Properties.Resources.plugin_description,
                    SubTitle = string.Format(CultureInfo.CurrentCulture, Properties.Resources.plugin_in_browser_name, BrowserInfo.Name ?? BrowserInfo.MSEdgeName),
                    QueryTextDisplay = string.Empty,
                    IcoPath = _iconPath,
                    ProgramArguments = arguments,
                    Action = action =>
                    {
                        if (!Helper.OpenCommandInShell(BrowserInfo.Path, BrowserInfo.ArgumentsPattern, arguments))
                        {
                            onPluginError();
                            return false;
                        }

                        return true;
                    },
                });
                return results;
            }
            else
            {
                string searchTerm = query.Search;

                try
                {
                    // Fetch response from ChatGPTService
                    string response = Task.Run(() => _chatGPTService.GetResponseAsync(searchTerm)).Result;

                    var result = new Result
                    {
                        Title = searchTerm,
                        SubTitle = response,
                        QueryTextDisplay = searchTerm,
                        IcoPath = _iconPath,
                        Action = _ => true
                    };

                    results.Add(result);
                }
                catch (Exception ex)
                {
                    // Log and handle errors gracefully
                    Log.Error($"Error fetching ChatGPT response: {ex.Message}", this.GetType());
                    _context.API.ShowMsg(
                        $"Plugin: {Properties.Resources.plugin_name}",
                        $"An error occurred while fetching the response: {ex.Message}");
                }
            }

            return results;
        }

        public void Init(PluginInitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(_context.API.GetCurrentTheme());
            BrowserInfo.UpdateIfTimePassed();

            onPluginError = () =>
            {
                string errorMsgString = string.Format(CultureInfo.CurrentCulture, Properties.Resources.plugin_search_failed, BrowserInfo.Name ?? BrowserInfo.MSEdgeName);

                Log.Error(errorMsgString, this.GetType());
                _context.API.ShowMsg(
                    $"Plugin: {Properties.Resources.plugin_name}",
                    errorMsgString);
            };
        }

        public string GetTranslatedPluginTitle()
        {
            return Properties.Resources.plugin_name;
        }

        public string GetTranslatedPluginDescription()
        {
            return Properties.Resources.plugin_description;
        }

        private void OnThemeChanged(Theme oldtheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        private void UpdateIconPath(Theme theme)
        {
            if (theme == Theme.Light || theme == Theme.HighContrastWhite)
            {
                _iconPath = "Images/ChatGPT.light.png";
            }
            else
            {
                _iconPath = "Images/ChatGPT.dark.png";
            }
        }

        public void ReloadData()
        {
            if (_context is null)
            {
                return;
            }

            UpdateIconPath(_context.API.GetCurrentTheme());
            BrowserInfo.UpdateIfTimePassed();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_context != null && _context.API != null)
                {
                    _context.API.ThemeChanged -= OnThemeChanged;
                }

                _disposed = true;
            }
        }
    }
}