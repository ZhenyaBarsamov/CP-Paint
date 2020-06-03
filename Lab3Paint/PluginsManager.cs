using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Lab3Paint {
    public class PluginsManager {
        private MainForm app; // главное окно приложения
        private ToolStrip tools; // панель инструментов главного окна

        // Словарь: название плагина - плагин
        private Dictionary<string, PaintInterfaces.IFilterPlugin> plugins = new Dictionary<string, PaintInterfaces.IFilterPlugin>();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="app">Главная форма приложения</param>
        /// <param name="tools">Ссылка на панель инструментов окна, на которой располагается меню плагинов</param>
        public PluginsManager(MainForm app, ToolStrip tools) {
            this.app = app;
            this.tools = tools;
        }

        private void OnPluginClick(object sender, EventArgs args) {
            var canvas = (Canvas)app.ActiveMdiChild;
            System.Drawing.Bitmap fBmp;

            //--- Через интерфейс:
            PaintInterfaces.IFilterPlugin plugin = (PaintInterfaces.IFilterPlugin)plugins[((ToolStripMenuItem)sender).Text];
            fBmp = plugin.TransformImage(canvas.GetCurImage());
            //---

            //--- Или через Invoke:
            //object pluginObj = plugins[((ToolStripMenuItem)sender).Text];
            //Type pluginType = pluginObj.GetType();
            //MethodInfo pluginMethod = pluginType.GetMethod("TransformImage");
            //fBmp = (System.Drawing.Bitmap)pluginMethod.Invoke(pluginObj, new object[] { canvas.GetCurImage() });
            //---

            canvas.SetFilteredImage(fBmp);
        }

        private void CreatePluginsMenu() {
            foreach (string name in plugins.Keys) {
                ToolStripMenuItem item = new ToolStripMenuItem(name);
                item.Click += OnPluginClick;
                ((ToolStripDropDownButton)tools.Items["toolStripDDBPlugins"]).DropDownItems.Add(item);
            }
        }

        /// <summary>
        /// Метод загрузки плагинов
        /// </summary>
        /// <returns>Возвращает значение успеха загрузки</returns>
        public bool LoadFilterPlugins() {
            // Папка с плагинами - текущая папка приложения
            string folder = System.AppDomain.CurrentDomain.BaseDirectory;

            // Получаем все dll-файлы в этой папке
            string[] files = Directory.GetFiles(folder, "*.dll");

            foreach (string file in files)
                try {
                    // Загружаем сборку
                    Assembly assembly = Assembly.LoadFile(file);

                    // Для всех типов в сборке проверяем, реализуют ли они интерфейс плагина-фильтра, и загружаем нужные
                    foreach (Type type in assembly.GetTypes()) {
                        Type iface = type.GetInterface("PaintInterfaces.IFilterPlugin");
                        
                        if (iface != null) {
                            PaintInterfaces.IFilterPlugin plugin = (PaintInterfaces.IFilterPlugin)Activator.CreateInstance(type);
                            plugins.Add(plugin.Name, plugin);
                        }
                    }
                }
                catch {
                    MessageBox.Show($"Ошибка при загрузке плагина {file}", "Ошибка загрузки плагинов", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

            // Создаём меню плагинов
            CreatePluginsMenu();
            return true;
        }
    }
}
