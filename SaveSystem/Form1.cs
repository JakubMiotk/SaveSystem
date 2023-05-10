using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveSystem
{
    public partial class Form1 : Form
    {
        public SaveStore savesList = new SaveStore();
        public string path = @"..\..\data\saves.json";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Załadowanie listy save'ów i dodanie ich do layout'u
            try { savesList = SaveStore.Load(path, "JSON"); } catch (ArgumentException) { MessageBox.Show(savesList.wrongFormatEx.Message); }
            
            foreach (string saveName in savesList.GetSaveNames())
            {
                SaveStore save = savesList.Get(saveName) as SaveStore;
                if (save != null)
                {
                    AddSaveToListView(save);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
                // Utwórz nowy Save
                SaveStore save = new SaveStore();
            try
            {
                save.Store("name", nameTextBox.Text);
                save.Store("level", levelTextBox.Text);
                save.Store("takeoff speed", TOTextBox.Text);
                save.Store("in-run speed", ISTextBox.Text);
                save.Store("balancing", BTextBox.Text);
                save.Store("positioning", PTextBox.Text);
                save.Store("landing", LTextBox.Text);
                save.Store("date", DateTime.Now);
                // Sprawdź, czy klucz już istnieje w słowniku
                if (savesList.Get(nameTextBox.Text) != null)
                {
                    MessageBox.Show($"Save with name: '{nameTextBox.Text}' already exist!");
                }
                // Jeśli nie - stwórz i dodaj do listy oraz layout'u
                else
                {
                    savesList.Store(nameTextBox.Text, save);
                    AddSaveToListView(save);
                    try { savesList.Save(path, "JSON"); MessageBox.Show("Save has been created."); } 
                    catch (ArgumentException) { MessageBox.Show(savesList.wrongFormatEx.Message); }              
                }
            }
            catch (KeyNotFoundException) 
            {
                MessageBox.Show(save.noValueEx.Message);
            };
                
        }
        private void AddSaveToListView(SaveStore save)
        {
            // Utwórz nowy element groupBox z Save'em i dodaj go do layout'u
            GroupBox saveBox = new GroupBox
            {
                Size = new Size(300, 250)
            };
            Label dateLabel = newLabel("date", new Point(20, 30), save);
            Label levelLabel = newLabel("level", new Point(20, 75), save);
            Label takeofflabel = newLabel("takeoff speed", new Point(20, 100), save);
            Label inrunLabel = newLabel("in-run speed", new Point(20, 125), save);
            Label balancinglabel = newLabel("balancing", new Point(20, 150), save);
            Label landingLabel = newLabel("landing", new Point(20, 175), save);
            Label positioningflabel = newLabel("positioning", new Point(20, 200), save);
            saveBox.Font = new Font("Arial", 12, FontStyle.Bold);
            saveBox.Text = save.Get("name").ToString();
            saveBox.Controls.Add(dateLabel);
            saveBox.Controls.Add(levelLabel);
            saveBox.Controls.Add(takeofflabel);
            saveBox.Controls.Add(inrunLabel);
            saveBox.Controls.Add(balancinglabel);
            saveBox.Controls.Add(positioningflabel);
            saveBox.Controls.Add(landingLabel);
            flowLayout.Controls.Add(saveBox);
        }
        private Label newLabel(string attribute, Point point, SaveStore save)
        {
            return new Label
            {
                Text = attribute + ": " + save.Get(attribute).ToString(),
                Location = point,
                Size = new Size(300, 20),
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
        }

    }
}
