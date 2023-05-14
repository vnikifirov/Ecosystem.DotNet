using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using corelib;
using RecoveryFactory;

namespace corelib
{
    class MainForm : Form
    {
        #region Automatic
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btAdd = new System.Windows.Forms.Button();
            this.btView = new System.Windows.Forms.Button();
            this.btRecover = new System.Windows.Forms.Button();
            this.btClear = new System.Windows.Forms.Button();
            this.btCreate = new System.Windows.Forms.Button();
            this.btCreateDatabase = new System.Windows.Forms.Button();
            this.btAzotEditor = new System.Windows.Forms.Button();
            this.btExportBase = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btAdd
            // 
            this.btAdd.Location = new System.Drawing.Point(47, 129);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(203, 42);
            this.btAdd.TabIndex = 2;
            this.btAdd.Text = "Добавить данные в базу...";
#if !DOTNET_V11
            this.btAdd.UseVisualStyleBackColor = true;
#endif
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // btView
            // 
            this.btView.Location = new System.Drawing.Point(47, 81);
            this.btView.Name = "btView";
            this.btView.Size = new System.Drawing.Size(203, 42);
            this.btView.TabIndex = 1;
            this.btView.Text = "Просмотр всей базы...";
#if !DOTNET_V11
            this.btView.UseVisualStyleBackColor = true;
#endif
            this.btView.Click += new System.EventHandler(this.btView_Click);
            // 
            // btRecover
            // 
            this.btRecover.Location = new System.Drawing.Point(47, 33);
            this.btRecover.Name = "btRecover";
            this.btRecover.Size = new System.Drawing.Size(203, 42);
            this.btRecover.TabIndex = 0;
            this.btRecover.Text = "Восстановление расходов..";
#if !DOTNET_V11
            this.btRecover.UseVisualStyleBackColor = true;
#endif
            this.btRecover.Click += new System.EventHandler(this.btRecover_Click);
            // 
            // btClear
            // 
            this.btClear.Enabled = false;
            this.btClear.Location = new System.Drawing.Point(47, 225);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(203, 42);
            this.btClear.TabIndex = 3;
            this.btClear.Text = "Очистить базу";
#if !DOTNET_V11
            this.btClear.UseVisualStyleBackColor = true;
#endif
            this.btClear.Visible = false;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // btCreate
            // 
            this.btCreate.Enabled = false;
            this.btCreate.Location = new System.Drawing.Point(47, 273);
            this.btCreate.Name = "btCreate";
            this.btCreate.Size = new System.Drawing.Size(203, 42);
            this.btCreate.TabIndex = 4;
            this.btCreate.Text = "Создать структуру базы";
#if !DOTNET_V11
            this.btCreate.UseVisualStyleBackColor = true;
#endif
            this.btCreate.Visible = false;
            this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
            // 
            // btCreateDatabase
            // 
            this.btCreateDatabase.Enabled = false;
            this.btCreateDatabase.Location = new System.Drawing.Point(47, 321);
            this.btCreateDatabase.Name = "btCreateDatabase";
            this.btCreateDatabase.Size = new System.Drawing.Size(203, 42);
            this.btCreateDatabase.TabIndex = 4;
            this.btCreateDatabase.Text = "Создать базу";
#if !DOTNET_V11
            this.btCreateDatabase.UseVisualStyleBackColor = true;
#endif
            this.btCreateDatabase.Visible = false;
            this.btCreateDatabase.Click += new System.EventHandler(this.btCreateDatabase_Click);
            // 
            // btAzotEditor
            // 
            this.btAzotEditor.Location = new System.Drawing.Point(47, 177);
            this.btAzotEditor.Name = "btAzotEditor";
            this.btAzotEditor.Size = new System.Drawing.Size(203, 42);
            this.btAzotEditor.TabIndex = 5;
            this.btAzotEditor.Text = "Редактор азотной активности...";
#if !DOTNET_V11
            this.btAzotEditor.UseVisualStyleBackColor = true;
#endif
            this.btAzotEditor.Click += new System.EventHandler(this.btAzotEditor_Click);
            // 
            // btExportBase
            // 
            this.btExportBase.Location = new System.Drawing.Point(47, 177);
            this.btExportBase.Name = "btAzotEditor";
            this.btExportBase.Size = new System.Drawing.Size(203, 42);
            this.btExportBase.TabIndex = 5;
            this.btExportBase.Text = "Экспорт всей базы...";
#if !DOTNET_V11
            this.btExportBase.UseVisualStyleBackColor = true;
#endif
            this.btExportBase.Visible = false;
            this.btExportBase.Click += new EventHandler(btExportBase_Click);
            // 
            // MainForm
            // 
#if !DOTNET_V11
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
#endif
            this.ClientSize = new System.Drawing.Size(310, 255);
            this.Controls.Add(this.btAzotEditor);
            this.Controls.Add(this.btCreateDatabase);
            this.Controls.Add(this.btCreate);
            this.Controls.Add(this.btClear);
            this.Controls.Add(this.btRecover);
            this.Controls.Add(this.btView);
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.btExportBase);
            this.Name = "MainForm";
            this.Text = "Задачи";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btView;
        private System.Windows.Forms.Button btRecover;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.Button btCreate;
        private System.Windows.Forms.Button btCreateDatabase;
        private System.Windows.Forms.Button btExportBase;

#endregion
        private Button btAzotEditor;
        private SunEnv _env;
        public MainForm(SunEnv env)
        {
            InitializeComponent();

            _env = env;
            if (!_env.Initialized)
                _env.Init();

            if (_env.RootParameters.GetParamSafe("SunEnv_ShowAdminButtons").IsNotNull)
            {
                string action = (string)_env.RootParameters["SunEnv_ShowAdminButtons"];
                if (action.ToLower() == "true")
                {
                    btClear.Enabled = true;
                    btClear.Visible = true;
                   
                    btCreateDatabase.Enabled = true;
                    btCreateDatabase.Visible = true;

                    btCreate.Enabled = true;
                    btCreate.Visible = true;
                }
            }
            if (_env.RootParameters.GetParamSafe("SunEnv_ShowExportButton").IsNotNull)
            {
                string action = (string)_env.RootParameters["SunEnv_ShowExportButton"];
                if (action.ToLower() == "true")
                {
                    btAzotEditor.Visible = false;
                    btExportBase.Visible = true;
                }
            }
        }

        private void btRecover_Click(object sender, EventArgs e)
        {
            try
            {
#if DEV                
                IMultiDataProvider rc = _env.GetDatabaseMainResource().GetMultiProvider();

                DataTupleVisualizer dv3 = new DataTupleVisualizer();
                dv3.SetEnviroment(_env);
                dv3.SetDataProvider(rc);
                dv3.Dock = DockStyle.Fill;

                Form dppppp3 = new Form();
                dppppp3.Controls.Add(dv3);
                dppppp3.Text = "Данные";
                dppppp3.ShowDialog(this); 
#else                
                
                RecoverySimple rs = new RecoverySimple(_env);
                rs.ShowDialog(this);
#endif                
            }
            catch { MessageBox.Show("Действие прервано", "Восстановление..."); }
        }

        private void btView_Click(object sender, EventArgs e)
        {
            try
            {
                _env.UpdateDates();
                _env.ViewMultiTuple(_env.GetAllData(_env.DefProvider));
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Действие прервано:\r\n{0}", ex.Message), 
                    "Просмотр...");
            }

        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            try
            {
                DTVSunEnvAdd.ImportToBase(_env, true);
            }
            catch (ActionCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Действие прервано:\r\n{0}", ex.Message), 
                    "Добавление...");
            }
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            DataComponents.DataResource mv = _env.GetDatabaseMainResource() as
                DataComponents.DataResource;

            if (mv != null)
            {
                AbstractSQLMultiProvider asql = mv.GetMultiProvider() as AbstractSQLMultiProvider;

                if (asql != null)
                {
                    try
                    {
                        asql.ClearAllData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "Ошибка при создании");
                    }

                    return;
                }
            }

            MessageBox.Show(this, "Не удается выполнить операцию");
        }


        private void btCreateDatabase_Click(object sender, EventArgs e)
        {
            DataComponents.DataResource mv = _env.GetDatabaseMainResource() as
                DataComponents.DataResource;

            if (mv != null)
            {
                AbstractSQLMultiProvider asql = mv.GetMultiProvider() as AbstractSQLMultiProvider;

                if (asql != null)
                {
                    try
                    {
                        string databaseName = (string)_env.RootParameters["SunEnv_CreateDatabaseName"];
                        asql.CreateDatabase(databaseName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "Ошибка при создании");
                    }

                    return;
                }
            }

            MessageBox.Show(this, "Не удается выполнить операцию");
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            DataComponents.DataResource mv = _env.GetDatabaseMainResource() as 
                DataComponents.DataResource;

            if (mv != null) {
                AbstractSQLMultiProvider asql = mv.GetMultiProvider() as AbstractSQLMultiProvider;

                if (asql != null)
                {
                    try
                    {
                        asql.CreateStructure();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "Ошибка при создании" );
                    }

                    return;
                }
            }

            MessageBox.Show(this, "Не удается выполнить операцию");
        }

        private void btAzotEditor_Click(object sender, EventArgs e)
        {
            try
            {
                AzotEditorPlugins.ShowAzotEditor(_env);
            }
            catch { MessageBox.Show("Действие прервано", "Восстановление..."); }
        }

        void btExportBase_Click(object sender, EventArgs e)
        {
            try
            {
                _env.ExportToBase();
            }
            catch { MessageBox.Show("Действие прервано", "Экспортирование..."); }
        }
    }
}