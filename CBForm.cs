using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace ACKS_CB
{
    public partial class CBForm : Form
    {
        private static Random rand = new Random(Guid.NewGuid().GetHashCode());
        private int statPoints = 0;
        private int lastReduced = -1;
        private charClass activeClass = new charClass();
        private ArrayList undoQueue;
        private int characterLevel;
        private string currentProf;
        private bool isClassProf;
        private int generalProfs;
        private int classProfs;
        private int classProfsTaken;
        private int generalProfsTaken;
        private ArrayList profTypeList;
        private int hp;
        private List<weapon> weapons = new List<weapon>();
        private List<armor> armors = new List<armor>();
        private List<item> items = new List<item>();

        public CBForm()
        {
            InitializeComponent();
            string[] files;
            files = System.IO.Directory.GetFiles(Application.StartupPath + "/Resources/Classes");
            string tester = "";
            foreach (string fileName in files)
            {
                tester = sanitizeClassName(fileName);
                if (tester != "") classesBox.Items.Add(sanitizeClassName(fileName));
            }
            undoQueue = new ArrayList();
            characterLevel = 0;
            generalProfs = 0;
            classProfs = 0;
            isClassProf = false;
            classProfsTaken = 0;
            generalProfsTaken = 0;
            profTypeList = new ArrayList();

            IntPtr h = this.tabControl1.Handle;

            populateGeneralProfBox();
            tabControl1.TabPages.Remove(castingPage);
            tabControl1.TabPages.Remove(templatePage);
            tabControl1.TabPages.Remove(shopPage);
            tabControl1.TabPages.Remove(thiefPage);

            updateProficiencyCount();
            rollerBox_Click(new Object(), new EventArgs());
            hp = 0;

            importWeapons();
            importArmors();
            importItems();

        }



        public void outputClass()
        {
            viewBox.Items.Add("Prime Requisite: " + activeClass.getPrimeReq());
            viewBox.Items.Add("Requirements: " + activeClass.getRequirements());
            viewBox.Items.Add("Maximum Level: " + activeClass.getMaxLevel());
            viewBox.Items.Add("Weapon Proficiencies: " + activeClass.getWeapons());
            viewBox.Items.Add("Armor Proficiencies: " + activeClass.getArmor());
            viewBox.Items.Add("Fighting Styles: " + activeClass.getStyles());
            outputClassValues();

            ArrayList xpValues = activeClass.getXPValues();
            ArrayList levelTitles = activeClass.getLevelTitles();
            int maxLevel = activeClass.getMaxLevel();
            ArrayList specials = activeClass.getSpecialsShort();
            ArrayList specialsLong = activeClass.getSpecialsLong();
            ArrayList profs = activeClass.getProfs();

            fillThievery();

            for (int i = 0; i < maxLevel; i++)
            {
                classGridView.Rows.Add(xpValues[i], levelTitles[i], i+1, getHDString(activeClass.getHD(), i+1), specials[i]);
                classGridView.Rows[i].Cells[4].ToolTipText = (string)specialsLong[i];
            }

            fillSpellcasting();

            int numItems = 0;
            numItems = proficiencyBox.Items.Count;

            for (int i = 0; i < numItems; i++)
            {
                proficiencyBox.Items.RemoveAt(0);
            }

            for (int i = 0; i < profs.Count; i++)
            {
                proficiencyBox.Items.Add(profs[i]);
            }

            fillThrowTable();
        }

        private void fillThrowTable()
        {
            //Clear the old table
            int numRows = throwDataTable.Rows.Count;

            for (int i = 0; i < numRows; i++)
            {
                throwDataTable.Rows.RemoveAt(0);
            }

            //Get a list for each saving throw type
            //Get the attack throw list
            //Add rows to the datagridview

            ArrayList attack = activeClass.getAttackThrowProgression();
            ArrayList petrif = activeClass.getSaves(0);
            ArrayList poison = activeClass.getSaves(1);
            ArrayList blast = activeClass.getSaves(2);
            ArrayList staff = activeClass.getSaves(3);
            ArrayList spells = activeClass.getSaves(4);

            //ELVES:  Bonus to petrif and spells.
            //DWARVES: Bonus to all saves.
            //BLESSED: Bonus to all saves.

            for (int i = 0; i < activeClass.getMaxLevel(); i++)
            {
                throwDataTable.Rows.Add(i + 1, petrif[i], poison[i], blast[i], staff[i], spells[i], attack[i]);
            }

        }

        private void fillThievery()
        {
            if (activeClass.getThievery() < 1)
            {
                tabControl1.TabPages.Remove(thiefPage);
                return;
            }
            else
            {
                if (!tabControl1.TabPages.Contains(thiefPage))
                    tabControl1.TabPages.Insert(tabControl1.TabPages.IndexOf(classPage) + 1, thiefPage);
            }

            //I have the page done!

            //Kill all the old rows
            int numRows = thiefGridView.Rows.Count;
            for (int i = 0; i < numRows; i++)
            {
                thiefGridView.Rows.RemoveAt(0);
            }

            //I need a class grid view here.
            //My goal:  For each thief skill the class actually has, add a column
            //Then for each column, fill it with stuff
            //This will require some doing.  I'll have to start by adding the empty rows then fill in the cells, I think, since I don't
            //know how many columns I'll have.
            //Hmm.  I wonder if I can build the column ahead of time, then add it when it's done?
            //Alternate method:  The table has everything in it, then I remove all the junk I don't want.

            System.IO.StreamReader thiefReader = new System.IO.StreamReader(Application.StartupPath + "/Resources/Classes/Thief Skills.txt");

            //Forgot I was just building the whole thing!
            /*for (int i = 0; i < thiefSkillNames.Count; i++)
            {
                thiefSkillsValues.Add(new ArrayList());
                while (line != (string)thiefSkillNames[i])
                {
                    line = charClass.read(thiefReader);
                }
                line = charClass.read(thiefReader);
                thiefSkillsValues[i] = activeClass.assembleThrows(line);
            }*/

            /*
             * Open Locks
             * Find Traps
             * Remove Traps
             * Pick Pockets
             * Move Silently
             * Climb Walls
             * Hide in Shadows
             * Hear Noise
             * */

            ArrayList open = getThiefProgression(thiefReader, "Open Locks");
            ArrayList find = getThiefProgression(thiefReader, "Find Traps");
            ArrayList remove = getThiefProgression(thiefReader, "Remove Traps");
            ArrayList pick = getThiefProgression(thiefReader, "Pick Pockets");
            ArrayList move = getThiefProgression(thiefReader, "Move Silently");
            ArrayList climb = getThiefProgression(thiefReader, "Climb Walls");
            ArrayList hide = getThiefProgression(thiefReader, "Hide In Shadows");
            ArrayList hear = getThiefProgression(thiefReader, "Hear Noise");

            for (int i = 0; i < activeClass.getMaxLevel(); i++)
            {
                thiefGridView.Rows.Add(i+1, open[i], find[i], remove[i], pick[i], move[i], climb[i], hide[i], hear[i]);
            }

            //Next step:  Remove the ones that the class does not have.
            //I start at 1 because columns[0] is level, and all classes have level.
            int columnCount = thiefGridView.Columns.Count;
            ArrayList thiefSkills = activeClass.getThiefSkills();
            List<DataGridViewColumn> toRemove = new List<DataGridViewColumn>();
            for (int i = 1; i < columnCount; i++)
            {
                if (!thiefSkills.Contains(thiefGridView.Columns[i].HeaderText))
                    toRemove.Add(thiefGridView.Columns[i]);
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                thiefGridView.Columns.Remove(toRemove[i]);
            }

        }

        private ArrayList getThiefProgression(System.IO.StreamReader reader, string name)
        {
            //Find the thief skill name in the reader
            //Then return the assembled throw of the next line
            string line = "";
            while (line != name && (!line.Contains("****")))
            {
                line = charClass.read(reader);
            }
            line = charClass.read(reader);
            return activeClass.assembleThrows(line);
        }

        private void fillSpellcasting()
        {
            //NOTE
            //The Elf Value dealy is super hacky.  I need to come up with a much better way than this to allow races to add to class values.
            //The problem is that I can't simply add to the value because that would mess up the automatic saving throw progression getter.
            int arcane = activeClass.getArcane() + activeClass.getElfValue(), divine = activeClass.getDivine(), psionic = activeClass.getPsionic();

            if ((arcane == 0) && (divine == 0) && (psionic == 0)) tabControl1.TabPages.Remove(castingPage);
            else if (!tabControl1.TabPages.Contains(castingPage))
            {
                tabControl1.TabPages.Insert(tabControl1.TabPages.IndexOf(classPage) + 1, castingPage);
            }

            fillArcaneSpellTable();
            fillDivineSpellTable();
            fillPsionicTable();
        }
        private void fillArcaneSpellTable()
        {
            int arcane = activeClass.getArcane() + activeClass.getElfValue();
            int arcaneDelayValue = activeClass.getArcaneDelay();
            List<List<string>> spells;

            if (arcaneDelayValue != 0)
            {
                spells = activeClass.delayedArcane();
            }
            else if (arcane == 0)
            {
                spells = new List<List<string>>();
                spells.Add(new List<string>());
            }
            else if (arcane == 1)
            {
                spells = activeClass.arcane1();
            }
            else if (arcane == 2)
            {
                spells = activeClass.arcane2();
            }
            else if (arcane == 3)
            {
                spells = activeClass.arcane3();
            }
            else if (arcane == 4)
            {
                spells = activeClass.arcane4();
            }
            else if (arcane == 5)
            {
                spells = activeClass.arcane5();
            }
            else if (arcane == 6)
            {
                spells = activeClass.arcane6();
            }
            else if (arcane == 7)
            {
                spells = activeClass.arcane7();
            }
            else if (arcane == 8)
            {
                spells = activeClass.arcane8();
            }
            else
            {
                spells = new List<List<string>>();
                spells.Add(new List<string>());
            }

            int numRows = arcaneTable.Rows.Count;

            for (int i = 0; i < numRows; i++)
            {
                arcaneTable.Rows.RemoveAt(0);
            }

            for (int i = 0; i < activeClass.getMaxLevel() && i < spells[0].Count; i++)
            {
                arcaneTable.Rows.Add(i+1, spells[0][i], spells[1][i], spells[2][i], spells[3][i], spells[4][i], spells[5][i]);
            }
        }

        private void fillDivineSpellTable()
        {
            int divine = activeClass.getDivine();
            int divineDelayValue = activeClass.getDivineDelay();
            List<List<string>> spells;

            if (divineDelayValue != 0)
            {
                spells = activeClass.delayedDivine();
            }
            else if (divine == 0)
            {
                spells = new List<List<string>>();
                spells.Add(new List<string>());
            }
            else if (divine == 1)
            {
                spells = activeClass.divine1();
            }
            else if (divine == 2)
            {
                spells = activeClass.divine2();
            }
            else if (divine == 3)
            {
                spells = activeClass.divine3();
            }
            else if (divine == 4)
            {
                spells = activeClass.divine4();
            }
            else if (divine == 5)
            {
                spells = activeClass.divine5();
            }
            else if (divine == 6)
            {
                spells = activeClass.divine6();
            }
            else if (divine == 7)
            {
                spells = activeClass.divine7();
            }
            else if (divine == 8)
            {
                spells = activeClass.divine8();
            }
            else
            {
                spells = new List<List<string>>();
                spells.Add(new List<string>());
            }

            int numRows = divineTable.Rows.Count;

            for (int i = 0; i < numRows; i++)
            {
                divineTable.Rows.RemoveAt(0);
            }

            for (int i = 0; i < activeClass.getMaxLevel() && i < spells[0].Count; i++)
            {
                divineTable.Rows.Add(i + 1, spells[0][i], spells[1][i], spells[2][i], spells[3][i], spells[4][i]);
            }
        }

        private void fillPsionicTable()
        {
            int psionic = activeClass.getPsionic();
            int psionicDelayValue = activeClass.getPsionicDelay();
            List<List<string>> powers;

            if (psionicDelayValue != 0)
            {
                powers = activeClass.delayedPsionic();
            }
            else if (psionic == 0)
            {
                powers = new List<List<string>>();
                powers.Add(new List<string>());
            }
            else if (psionic == 1)
            {
                powers = activeClass.psionic1();
            }
            else if (psionic == 2)
            {
                powers = activeClass.psionic2();
            }
            else if (psionic == 3)
            {
                powers = activeClass.psionic3();
            }
            else if (psionic == 4)
            {
                powers = activeClass.psionic4();
            }
            else if (psionic == 5)
            {
                powers = activeClass.psionic5();
            }
            else if (psionic == 6)
            {
                powers = activeClass.psionic6();
            }
            else if (psionic == 7)
            {
                powers = activeClass.psionic7();
            }
            else if (psionic == 8)
            {
                powers = activeClass.psionic8();
            }
            else
            {
                powers = new List<List<string>>();
                powers.Add(new List<string>());
            }

            int numRows = psionicDataTable.Rows.Count;

            for (int i = 0; i < numRows; i++)
            {
                psionicDataTable.Rows.RemoveAt(0);
            }

            for (int i = 0; i < activeClass.getMaxLevel() && i < powers[0].Count; i++)
            {
                psionicDataTable.Rows.Add(i + 1, powers[0][i], powers[1][i], powers[2][i], powers[3][i], activeClass.getPSPS(System.Convert.ToInt32(intMod.Text), System.Convert.ToInt32(wisMod.Text), System.Convert.ToInt32(conMod.Text), i+1));
            }
        }

        private string getHDString(int hd, int level)
        {
            string dice = "";

            int multiplier = getMultiplier();
            switch (hd)
            {
                case 0:
                    dice = "d4";
                    break;
                case 1:
                    dice = "d6";
                    break;
                case 2:
                    dice = "d8";
                    break;
                case 3:
                    dice = "d10";
                    break;
                case 4:
                    dice = "d12";
                    break;
            }
            if (level < 10) return System.Convert.ToString(level) + dice;
            else return "9" + dice + "+" + System.Convert.ToString((level - 9) * multiplier);
        }

        private int getMultiplier()
        {
            int returner = 0;
            if ((activeClass.getSaveProgression() == "Cleric") || (activeClass.getSaveProgression() == "Mage"))
                returner =  1;
            else if ((activeClass.getSaveProgression() == "Fighter") || (activeClass.getSaveProgression() == "Thief"))
                returner =  2;
            if (activeClass.dwarf) returner++;
            return returner;
        }

        private void outputClassValues()
        {
            int hd = activeClass.getHD();
            int fighting = activeClass.getFighting();
            int arcane = activeClass.getArcane();
            int divine = activeClass.getDivine();
            int thievery = activeClass.getThievery();
            int psionic = activeClass.getPsionic();

            if (hd > 0) viewBox.Items.Add("Hit Dice Value: " + hd);
            if (fighting > 0) viewBox.Items.Add("Fighting Value: " + fighting);
            if (arcane > 0) viewBox.Items.Add("Arcane Value: " + arcane);
            if (divine > 0) viewBox.Items.Add("Divine Value: " + divine);
            if (thievery > 0) viewBox.Items.Add("Thievery Value: " + thievery);
            if (psionic > 0) viewBox.Items.Add("Psionic Value: " + psionic);
        }

        private string sanitizeClassName(string raw)
        {
            if (raw.Contains("Proficiencies.txt") || (raw.Contains("Saves.txt")) || (raw.Contains("Attacks.txt")) || (raw.Contains("Template.txt")) || (raw.Contains("Thief Skills.txt"))) return "";
            string temp = raw.Substring(raw.IndexOf("/Resources/Classes"));
            temp = temp.Substring(temp.IndexOf("\\") +1);
            return temp.Substring(0, temp.Length - 4);
        }

        private void rollerBox_Click(object sender, EventArgs e)
        {
            int roll = 0;
            for (int i = 0; i < 6; i++)
            {
                roll = rand.Next(1, 7) + rand.Next(1, 7) + rand.Next(1, 7);
                assignStat(i, roll);
            }
            statPoints = 0;
            pointBox.Text = "0";

            strUp.Enabled = false;
            intUp.Enabled = false;
            conUp.Enabled = false;
            dexUp.Enabled = false;
            wisUp.Enabled = false;
            chaUp.Enabled = false;
        }

        private void assignStat(int stat, int roll)
        {
            if ((stat < 0) || (stat > 5)) return;
            switch (stat)
            {
                case 0:
                    strBox.Text = System.Convert.ToString(roll);
                    break;
                case 1:
                    intBox.Text = System.Convert.ToString(roll);
                    break;
                case 2:
                    wisBox.Text = System.Convert.ToString(roll);
                    break;
                case 3:
                    dexBox.Text = System.Convert.ToString(roll);
                    break;
                case 4:
                    conBox.Text = System.Convert.ToString(roll);
                    break;
                case 5:
                    chaBox.Text = System.Convert.ToString(roll);
                    break;
            }
        }

        private string getMod(int stat)
        {
            //3 : -3
            //4-5: -2
            //6-8: -1
            //9-12: 0
            //13-15: +1
            //16-17: +2
            //18: +3
            if ((stat < 3) || (stat > 18)) return "00";
            if (stat == 3)
            {
                return "-3";
            }
            else if (stat < 6)
            {
                return "-2";
            }
            else if (stat < 9)
            {
                return "-1";
            }
            else if (stat < 13)
            {
                return "0";
            }
            else if (stat < 16)
            {
                return "+1";
            }
            else if (stat < 18)
            {
                return "+2";
            }
            else
            {
                return "+3";
            }
        }

        private void strBox_TextChanged(object sender, EventArgs e)
        {
            if (strBox.Text == "") return;
            strMod.Text = System.Convert.ToString(getMod(System.Convert.ToInt32(strBox.Text)));
            strDown.Enabled = (System.Convert.ToInt32(strBox.Text) > 11);
            strUp.Enabled = (System.Convert.ToInt32(strBox.Text) < 18);
        }

        private void intBox_TextChanged(object sender, EventArgs e)
        {
            if (intBox.Text == "") return;
            intMod.Text = getMod(System.Convert.ToInt32(intBox.Text));
            intDown.Enabled = (System.Convert.ToInt32(intBox.Text) > 11);
            intUp.Enabled = (System.Convert.ToInt32(intBox.Text) < 18);
        }

        private void wisBox_TextChanged(object sender, EventArgs e)
        {
            if (wisBox.Text == "") return;
            wisMod.Text = getMod(System.Convert.ToInt32(wisBox.Text));
            wisDown.Enabled = (System.Convert.ToInt32(wisBox.Text) > 11);
            wisUp.Enabled = (System.Convert.ToInt32(wisBox.Text) < 18);
        }

        private void dexBox_TextChanged(object sender, EventArgs e)
        {
            if (dexBox.Text == "") return;
            dexMod.Text = getMod(System.Convert.ToInt32(dexBox.Text));
            dexDown.Enabled = (System.Convert.ToInt32(dexBox.Text) > 11);
            dexUp.Enabled = (System.Convert.ToInt32(dexBox.Text) < 18);
        }

        private void conBox_TextChanged(object sender, EventArgs e)
        {
            if (conBox.Text == "") return;
            conMod.Text = getMod(System.Convert.ToInt32(conBox.Text));
            conDown.Enabled = (System.Convert.ToInt32(conBox.Text) > 11);
            conUp.Enabled = (System.Convert.ToInt32(conBox.Text) < 18);
            if (characterLevel > 0) hp = rollHitPoints();
        }

        private void chaBox_TextChanged(object sender, EventArgs e)
        {
            if (chaBox.Text == "") return;
            chaMod.Text = getMod(System.Convert.ToInt32(chaBox.Text));
            chaDown.Enabled = (System.Convert.ToInt32(chaBox.Text) > 11);
            chaUp.Enabled = (System.Convert.ToInt32(chaBox.Text) < 18);
        }

        private void strDown_Click(object sender, EventArgs e)
        {
            strBox.Text = System.Convert.ToString(System.Convert.ToInt32(strBox.Text) - 2);
            statPoints++;
            pointBox.Text = System.Convert.ToString(statPoints);
            //lastReduced = 0;
            undoQueue.Add(0);
        }

        private void intDown_Click(object sender, EventArgs e)
        {
            intBox.Text = System.Convert.ToString(System.Convert.ToInt32(intBox.Text) - 2);
            statPoints++;
            pointBox.Text = System.Convert.ToString(statPoints);
            //lastReduced = 1;
            undoQueue.Add(1);
        }

        private void wisDown_Click(object sender, EventArgs e)
        {
            wisBox.Text = System.Convert.ToString(System.Convert.ToInt32(wisBox.Text) - 2);
            statPoints++;
            pointBox.Text = System.Convert.ToString(statPoints);
            //lastReduced = 2;
            undoQueue.Add(2);
        }

        private void dexDown_Click(object sender, EventArgs e)
        {
            dexBox.Text = System.Convert.ToString(System.Convert.ToInt32(dexBox.Text) - 2);
            statPoints++;
            pointBox.Text = System.Convert.ToString(statPoints);
            //lastReduced = 3;
            undoQueue.Add(3);
        }

        private void conDown_Click(object sender, EventArgs e)
        {
            conBox.Text = System.Convert.ToString(System.Convert.ToInt32(conBox.Text) - 2);
            statPoints++;
            pointBox.Text = System.Convert.ToString(statPoints);
            //lastReduced = 4;
            undoQueue.Add(4);
        }

        private void chaDown_Click(object sender, EventArgs e)
        {
            chaBox.Text = System.Convert.ToString(System.Convert.ToInt32(chaBox.Text) - 2);
            statPoints++;
            pointBox.Text = System.Convert.ToString(statPoints);
            //lastReduced = 5;
            undoQueue.Add(5);
        }

        private void pointBox_TextChanged(object sender, EventArgs e)
        {
            strUp.Enabled = (statPoints > 0 && System.Convert.ToInt32(strBox.Text) < 18);
            intUp.Enabled = (statPoints > 0 && System.Convert.ToInt32(intBox.Text) < 18);
            wisUp.Enabled = (statPoints > 0 && System.Convert.ToInt32(wisBox.Text) < 18);
            dexUp.Enabled = (statPoints > 0 && System.Convert.ToInt32(dexBox.Text) < 18);
            conUp.Enabled = (statPoints > 0 && System.Convert.ToInt32(conBox.Text) < 18);
            chaUp.Enabled = (statPoints > 0 && System.Convert.ToInt32(chaBox.Text) < 18);
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            if (undoQueue.Count == 0) return;
            lastReduced = (int)undoQueue[undoQueue.Count - 1];
            undoQueue.RemoveAt(undoQueue.Count - 1);
            if (lastReduced > 6) return;
            switch (lastReduced)
            {
                case -6:
                    statPoints++;
                    chaBox.Text = System.Convert.ToString(System.Convert.ToInt32(chaBox.Text) - 1);
                    break;
                case -5:
                    statPoints++;
                    conBox.Text = System.Convert.ToString(System.Convert.ToInt32(conBox.Text) - 1);
                    break;
                case -4:
                    statPoints++;
                    dexBox.Text = System.Convert.ToString(System.Convert.ToInt32(dexBox.Text) - 1);
                    break;
                case -3:
                    statPoints++;
                    wisBox.Text = System.Convert.ToString(System.Convert.ToInt32(wisBox.Text) - 1);
                    break;
                case -2:
                    statPoints++;
                    intBox.Text = System.Convert.ToString(System.Convert.ToInt32(intBox.Text) - 1);
                    break;
                case -1:
                    statPoints++;
                    strBox.Text = System.Convert.ToString(System.Convert.ToInt32(strBox.Text) - 1);
                    break;
                case 0:
                    statPoints--;
                    strBox.Text = System.Convert.ToString(System.Convert.ToInt32(strBox.Text) + 2);
                    break;
                case 1:
                    statPoints--;
                    intBox.Text = System.Convert.ToString(System.Convert.ToInt32(intBox.Text) + 2);
                    break;
                case 2:
                    statPoints--;
                    wisBox.Text = System.Convert.ToString(System.Convert.ToInt32(wisBox.Text) + 2);
                    break;
                case 3:
                    statPoints--;
                    dexBox.Text = System.Convert.ToString(System.Convert.ToInt32(dexBox.Text) + 2);
                    break;
                case 4:
                    statPoints--;
                    conBox.Text = System.Convert.ToString(System.Convert.ToInt32(conBox.Text) + 2);
                    break;
                case 5:
                    statPoints--;
                    chaBox.Text = System.Convert.ToString(System.Convert.ToInt32(chaBox.Text) + 2);
                    break;
            }
            pointBox.Text = System.Convert.ToString(statPoints);
        }

        private void strUp_Click(object sender, EventArgs e)
        {
            strBox.Text = System.Convert.ToString(System.Convert.ToInt32(strBox.Text) + 1);
            statPoints--;
            pointBox.Text = System.Convert.ToString(statPoints);
            undoQueue.Add(-1);
        }

        private void intUp_Click(object sender, EventArgs e)
        {
            intBox.Text = System.Convert.ToString(System.Convert.ToInt32(intBox.Text) + 1);
            statPoints--;
            pointBox.Text = System.Convert.ToString(statPoints);
            undoQueue.Add(-2);
        }

        private void wisUp_Click(object sender, EventArgs e)
        {
            wisBox.Text = System.Convert.ToString(System.Convert.ToInt32(wisBox.Text) + 1);
            statPoints--;
            pointBox.Text = System.Convert.ToString(statPoints);
            undoQueue.Add(-3);
        }

        private void dexUp_Click(object sender, EventArgs e)
        {
            dexBox.Text = System.Convert.ToString(System.Convert.ToInt32(dexBox.Text) + 1);
            statPoints--;
            pointBox.Text = System.Convert.ToString(statPoints);
            undoQueue.Add(-4);
        }

        private void conUp_Click(object sender, EventArgs e)
        {
            conBox.Text = System.Convert.ToString(System.Convert.ToInt32(conBox.Text) + 1);
            statPoints--;
            pointBox.Text = System.Convert.ToString(statPoints);
            undoQueue.Add(-5);
        }

        private void chaUp_Click(object sender, EventArgs e)
        {
            chaBox.Text = System.Convert.ToString(System.Convert.ToInt32(chaBox.Text) + 1);
            statPoints--;
            pointBox.Text = System.Convert.ToString(statPoints);
            undoQueue.Add(-6);
        }

        private void classesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            activeClass = new charClass(classesBox.Text);
            int numItems = viewBox.Items.Count;
            for (int i = 0; i < numItems; i++)
            {
                viewBox.Items.RemoveAt(0);
            }

            numItems = classGridView.Rows.Count;

            for (int i = 0; i < numItems; i++)
            {
                classGridView.Rows.RemoveAt(0);
            }

            outputClass();
            characterLevel = 1;
            levelBox.Text = "1";
            classGridView.Rows[0].Selected = true;
            hp = rollHitPoints();

            //empty proficiency box as well
            int takenBoxItems = takenBox.Items.Count;
            classProfsTaken = 0;
            generalProfsTaken = 0;
            for (int i = 0; i < takenBoxItems; i++)
            {
                takenBox.Items.RemoveAt(0);
            }

        }

        private void classGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            characterLevel = e.RowIndex + 1;
            if (characterLevel > 0) hp = rollHitPoints();
            levelBox.Text = System.Convert.ToString(e.RowIndex + 1);
            updateProficiencyCount();
        }

        private void classGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            characterLevel = e.RowIndex + 1;
            if (characterLevel > 0) hp = rollHitPoints();
            levelBox.Text = System.Convert.ToString(e.RowIndex + 1);
            updateProficiencyCount();
        }

        private void proficiencyBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (proficiencyBox.SelectedIndex > -1)
                currentProf = (string)proficiencyBox.Items[proficiencyBox.SelectedIndex];
            else currentProf = "";
            isClassProf = true;

            int numItems = descriptionBox.Items.Count;
            for (int i = 0; i < numItems; i++)
            {
                descriptionBox.Items.RemoveAt(0);
            }

            ArrayList description = activeClass.getDescription(currentProf);

            for (int i = 0; i < description.Count; i++)
            {
                descriptionBox.Items.Add(description[i]);
            }
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            //takenBox.Items.Add(currentProf);
            if (isClassProf && (classProfsTaken < System.Convert.ToInt32(classProfsBox.Text)))
            {
                takenBox.Items.Add(currentProf);
                profTypeList.Add(isClassProf);
                classProfsTaken++;
            }
            else if (!isClassProf && (generalProfsTaken < System.Convert.ToInt32(generalProfsBox.Text)))
            {
                takenBox.Items.Add(currentProf);
                profTypeList.Add(isClassProf);
                generalProfsTaken++;
            }
                //Is class prof, over limit
            else if (isClassProf)
            {
                MessageBox.Show("You have selected all of your class proficiencies!");
            }
                //Is general prof, over limit
            else
            {
                MessageBox.Show("You have selected all of your general proficiencies!");
            }
        }

        private void unselectButton_Click(object sender, EventArgs e)
        {
            if ((bool)profTypeList[takenBox.SelectedIndex])
            {
                classProfsTaken--;
            }
            else generalProfsTaken--;
            profTypeList.RemoveAt(takenBox.SelectedIndex);
            takenBox.Items.RemoveAt(takenBox.SelectedIndex);
        }

        private void generalProfBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentProf = (string)generalProfBox.Items[generalProfBox.SelectedIndex];
            isClassProf = false;
            int numItems = descriptionBox.Items.Count;
            for (int i = 0; i < numItems; i++)
            {
                descriptionBox.Items.RemoveAt(0);
            }

            ArrayList description = activeClass.getDescription(currentProf);

            for (int i = 0; i < description.Count; i++)
            {
                descriptionBox.Items.Add(description[i]);
            }
        }


        private void populateGeneralProfBox()
        {
            /*
             * General Proficiency List:Adventuring, Alchemy, Animal 
             * Husbandry, Animal Training, Art, Bargaining, Caving, 
             * Collegiate Wizardry, Craft, Diplomacy, Disguise, Endurance, 
             * Engineering, Gambling, Healing, Intimidation, Knowledge, 
             * Labor, Language, Leadership, Lip Reading, Manual of Arms, 
             * Mapping, Military Strategy, Mimicry, Naturalism, Navigation, 
             * Performance, Profession, Riding, Seafaring, Seduction, Siege 
             * Engineering, Signaling, Survival, Theology, Tracking, Trapping
             * */
            generalProfBox.Items.Add("Adventuring");
            generalProfBox.Items.Add("Alchemy");
            generalProfBox.Items.Add("Animal Husbandry");
            generalProfBox.Items.Add("Animal Training");
            generalProfBox.Items.Add("Art");
            generalProfBox.Items.Add("Bargaining");
            generalProfBox.Items.Add("Caving");
            generalProfBox.Items.Add("Collegiate Wizardry");
            generalProfBox.Items.Add("Craft");
            generalProfBox.Items.Add("Diplomacy");
            generalProfBox.Items.Add("Disguise");
            generalProfBox.Items.Add("Endurance");
            generalProfBox.Items.Add("Engineering");
            generalProfBox.Items.Add("Gambling");
            generalProfBox.Items.Add("Healing");
            generalProfBox.Items.Add("Intimidation");
            generalProfBox.Items.Add("Knowledge");
            generalProfBox.Items.Add("Labor");
            generalProfBox.Items.Add("Language");
            generalProfBox.Items.Add("Leadership");
            generalProfBox.Items.Add("Lip Reading");
            generalProfBox.Items.Add("Manual of Arms");
            generalProfBox.Items.Add("Mapping");
            generalProfBox.Items.Add("Military Strategy");
            generalProfBox.Items.Add("Mimicry");
            generalProfBox.Items.Add("Naturalism");
            generalProfBox.Items.Add("Navigation");
            generalProfBox.Items.Add("Performance");
            generalProfBox.Items.Add("Profession");
            generalProfBox.Items.Add("Riding");
            generalProfBox.Items.Add("Seafaring");
            generalProfBox.Items.Add("Seduction");
            generalProfBox.Items.Add("Siege Engineering");
            generalProfBox.Items.Add("Signaling");
            generalProfBox.Items.Add("Survival");
            generalProfBox.Items.Add("Theology");
            generalProfBox.Items.Add("Tracking");
            generalProfBox.Items.Add("Trapping");
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Application.StartupPath;


            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            //Code to actually write the file goes here
            string writerText = "";
            for (int i = 0; i < charSheetBox.Items.Count; i++)
            {
                writerText += charSheetBox.Items[i];
                writerText += Environment.NewLine;
            }
            System.IO.File.WriteAllText(saveFileDialog1.FileName, writerText);
        }

        private void levelBox_Update(object sender, EventArgs e)
        {
            if (levelBox.Items.Contains(levelBox.Text))
            {
                if (System.Convert.ToInt32(levelBox.Text) < classGridView.Rows.Count)
                {
                    for (int i = 0; i < classGridView.Rows.Count; i++)
                    {
                        classGridView.Rows[i].Selected = false;
                    }
                    classGridView.Rows[System.Convert.ToInt32(levelBox.Text)-1].Selected = true;
                }
                characterLevel = System.Convert.ToInt32(levelBox.Text);
                if (characterLevel > 0) hp = rollHitPoints();
                updateProficiencyCount();
            }
            
        }

        private void updateProficiencyCount()
        {
            //PROFICIENCY PROGRESSION
            //1 GENERAL, 1 CLASS AT LEVEL 1
            //PLUS INT MOD GENERAL PROFS
            //PLUS ONE GENERAL AT 5, 9, 13
            //PLUS CLASS BASED ON SAVE PROGRESSION

            //LEVEL 1: ONE OF EACH
            classProfs = 1;
            generalProfs = 1;

            //Add Int mod to General
            if (System.Convert.ToInt32(intMod.Text) > 0)
            generalProfs += System.Convert.ToInt32(intMod.Text);

            //Add general profs at 5, 9, and 13; this is equal to level -1, divided by 4, truncated.  We'll see if the base int math works for it.
            generalProfs += ((characterLevel - 1) / 4);

            string temp = activeClass.getSaveProgression() + "ClassCount";
            System.Reflection.MethodInfo methodinfo = identifyFunction(temp);
            if (methodinfo != null)
                classProfs += (int)methodinfo.Invoke(this, null);

            generalProfsBox.Text = System.Convert.ToString(generalProfs);
            classProfsBox.Text = System.Convert.ToString(classProfs);

        }

        public System.Reflection.MethodInfo identifyFunction(string temp)
        {
            Type type = typeof(CBForm);
            return type.GetMethod(temp);
        }

        public int FighterClassCount()
        {
            //Fighters get 1 class proficiency at 3, 6, 9, 12
            return characterLevel / 3;
        }

        public int MageClassCount()
        {
            //Mages get 1 class prof at 6 and 12
            return characterLevel / 6;
        }

        public int ClericClassCount()
        {
            //Clerics are 4, 8, 12
            return characterLevel / 4;
        }

        public int ThiefClassCount()
        {
            //Thieves are also 4, 8, 12
            return characterLevel / 4;
        }

        private void intMod_TextChanged(object sender, EventArgs e)
        {
            updateProficiencyCount();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == tabControl1.TabPages.IndexOf(outputPage))
            {
                outputCharacterSheet();
            }
            else if (tabControl1.SelectedIndex == tabControl1.TabPages.IndexOf(equipPage))
            {
                updateEquipPage();
            }
        }

        
        private void outputCharacterSheet()
        {
            if (characterLevel < 1) return;
            //charSheetBox.Text
            int numItems = charSheetBox.Items.Count;
            for (int i = 0; i < numItems; i++)
            {
                charSheetBox.Items.RemoveAt(0);
            }
            /*
             * Name
             * Class
             * Level - Level Title
             * Strength
             * Intelligence
             * Constitution
             * Dexterity
             * Wisdom
             * Charisma
             * Hit Dice/Hit Points
             * Movement Speed
             * Proficiencies
             * Attack Throws and Saving Throws
             * Gear List
             * */
            charSheetBox.Items.Add("Name: ");
            charSheetBox.Items.Add("Class: " + classesBox.Text);
            charSheetBox.Items.Add("Level: " + characterLevel + " - " + classGridView.Rows[characterLevel - 1].Cells[1].Value.ToString());
            charSheetBox.Items.Add("");
            charSheetBox.Items.Add("Strength: " + strBox.Text + "(" + strMod.Text + ")");
            charSheetBox.Items.Add("Intelligence: " + intBox.Text + "(" + intMod.Text + ")");
            charSheetBox.Items.Add("Constitution: " + conBox.Text + "(" + conMod.Text + ")");
            charSheetBox.Items.Add("Dexterity: " + dexBox.Text + "(" + dexMod.Text + ")");
            charSheetBox.Items.Add("Wisdom: " + wisBox.Text + "(" + wisMod.Text + ")");
            charSheetBox.Items.Add("Charisma: " + chaBox.Text + "(" + chaMod.Text + ")");
            charSheetBox.Items.Add("");
            charSheetBox.Items.Add("Hit Dice: " + getHitDice());
            charSheetBox.Items.Add("Hit Points: " + hp);
            charSheetBox.Items.Add("AC: " + getAC());
            charSheetBox.Items.Add("Movement Rate: " + getMovementSpeed());
            charSheetBox.Items.Add("");
            charSheetBox.Items.Add("Proficiencies");
            writeProficiencies();
            charSheetBox.Items.Add("");
            charSheetBox.Items.Add("Attacks: ");
            outputAttacks();
            charSheetBox.Items.Add("");
            charSheetBox.Items.Add("Attack and Saving Throws");
            //Petrification & Paralysis, Poison & Death, Blast & Breath, Staff & Wand, Spells, Attack Throw
            charSheetBox.Items.Add("Petrification & Paralysis: " + throwDataTable.Rows[characterLevel - 1].Cells[1].Value.ToString());
            charSheetBox.Items.Add("Poison & Death: " + throwDataTable.Rows[characterLevel - 1].Cells[2].Value.ToString());
            charSheetBox.Items.Add("Blast & Breath: " + throwDataTable.Rows[characterLevel - 1].Cells[3].Value.ToString());
            charSheetBox.Items.Add("Staff & Wand: " + throwDataTable.Rows[characterLevel - 1].Cells[4].Value.ToString());
            charSheetBox.Items.Add("Spells: " + throwDataTable.Rows[characterLevel - 1].Cells[5].Value.ToString());
            charSheetBox.Items.Add("Attack Throw: " + throwDataTable.Rows[characterLevel - 1].Cells[6].Value.ToString());
            charSheetBox.Items.Add("");
            outputThievery();

            charSheetBox.Items.Add("Gear");
            charSheetBox.Items.Add(gpBox.Text + " gp");
            outputWeapons();
            outputArmor();
            outputItems();

        }

        private void outputThievery()
        {
            if (activeClass.getThievery() > 0)
            {
                charSheetBox.Items.Add("Thief Skill Throws");
                //Since columns are not fixed the way they are for the attack and saving throws, this is slightly more complicated
                //I start at 1 instead of 0 so I don't output my level
                for (int i = 1; i < thiefGridView.Columns.Count; i++)
                {
                    //For each column:
                    //Output the header text, then the cell at current level
                    charSheetBox.Items.Add(thiefGridView.Columns[i].HeaderText + " " + thiefGridView.Rows[characterLevel - 1].Cells[i].Value.ToString());
                }
            }
        }

        private void outputWeapons()
        {
            //for (int i = 0; i < weaponOwnedBox.Items.Count; i++)
            //{
            //    charSheetBox.Items.Add(weaponOwnedBox.Items[i]);
            //}
            outputListBox(weaponOwnedBox);
        }

        private void outputArmor()
        {
            outputListBox(armorOwnedBox);
        }

        private void outputItems()
        {
            outputListBox(itemOwnedBox);
        }

        private void outputListBox(ListBox output)
        {
            for (int i = 0; i < output.Items.Count; i++)
            {
                charSheetBox.Items.Add(output.Items[i]);
            }
        }

        private void outputAttacks()
        {
            bool outputSecondWeapon = false;
            weapon currentWeapon = new weapon();
            if (weaponEquipBox.Text != "")
            {
                //string weaponName = weaponEquipBox.Text;
                //for (int i = 0; i < weapons.Count; i++)
                //{
                   // if (weapons[i].name == weaponName)
                    //{
                    //    currentWeapon = weapons[i];
                    //}
                //}
                currentWeapon = findWeapon(weaponEquipBox.Text);

                //Check for dual wield.  How do I check for dual wield?  If two melee weapons are equipped and neither is a two-hander.
                //Hmm.  This might be too complicated.  Alternate method:  Dual wield checkbox.
                if (dualWieldBox.Checked)
                {
                    //Dual wield is exactly the same as using a single one-handed weapon, with +1 to hit.
                    string temp = getModAttackThrow(currentWeapon, System.Convert.ToString(System.Convert.ToInt32(throwDataTable.Rows[characterLevel - 1].Cells[6].Value.ToString().Substring(0, throwDataTable.Rows[characterLevel - 1].Cells[6].Value.ToString().IndexOf("+")))-1) + "+");
                    charSheetBox.Items.Add(currentWeapon.name + ": " + temp + ", " + getModDamage(currentWeapon.range, currentWeapon.damage.Substring(0, currentWeapon.damage.IndexOf('/'))));
                }
                //Might do extra damage if used two-handed.
                else if (currentWeapon.damage.Contains("/"))
                {
                    //Is being used two-handed
                    if (weaponEquipBox.Text == weaponEquipBox2.Text)
                    {
                        //charSheetBox.Items.Add(weaponName + ": " + throwDataTable.Rows[characterLevel - 1].Cells[6].Value.ToString() + ", " + currentWeapon.damage.Substring(currentWeapon.damage.IndexOf('/') + 1));
                        charSheetBox.Items.Add(currentWeapon.name + ": " + getModAttackThrow(currentWeapon, throwDataTable.Rows[characterLevel - 1].Cells[6].Value.ToString()) + ", " +  getModDamage(currentWeapon.range, currentWeapon.damage.Substring(currentWeapon.damage.IndexOf('/') + 1))  );
                    }
                    //Is being used one-handed
                    else
                    {
                        //Might be dual-wielding if there is a second melee weapon in the off hand
                        //For now, though, just pretend it's being used for a different attack routine
                        outputSecondWeapon = true;
                        charSheetBox.Items.Add(currentWeapon.name + ": " + getModAttackThrow(currentWeapon, throwDataTable.Rows[characterLevel - 1].Cells[6].Value.ToString()) + ", " + getModDamage(currentWeapon.range, currentWeapon.damage.Substring(0, currentWeapon.damage.IndexOf('/'))));
                    }
                }
                //Does not do extra damage when used two-handed.
                else
                {
                    //Might be dual-wielding if there is a second melee weapon in the off hand
                    //For now, though, just pretend it's being used for a different attack routine

                    //Forgot to check for two-handed weapon!

                    if (currentWeapon.damage == "1d10")
                    {
                        charSheetBox.Items.Add(currentWeapon.name + ": " + getModAttackThrow(currentWeapon, throwDataTable.Rows[characterLevel - 1].Cells[6].Value.ToString()) + ", " + getModDamage(currentWeapon.range, currentWeapon.damage));
                    }
                    else
                    {
                        outputSecondWeapon = true;
                        charSheetBox.Items.Add(currentWeapon.name + ": " + getModAttackThrow(currentWeapon, throwDataTable.Rows[characterLevel - 1].Cells[6].Value.ToString()) + ", " + getModDamage(currentWeapon.range, currentWeapon.damage));
                    }
                }
            }

            if (weaponEquipBox2.Text == "Shield")
                outputSecondWeapon = false;

            if ((outputSecondWeapon) && (weaponEquipBox2.Text != null))
            {
                //The second weapon is definitely not being used two-handed
                string weaponName = weaponEquipBox2.Text;
                if (weaponName != "")
                {
                    //for (int i = 0; i < weapons.Count; i++)
                    //{
                       // if (weapons[i].name == weaponName)
                       // {
                         //   currentWeapon = weapons[i];
                        //}
                    //}
                    currentWeapon = findWeapon(weaponName);

                    if (currentWeapon.damage.Contains("/"))
                        charSheetBox.Items.Add(weaponName + ": " + getModAttackThrow(currentWeapon, throwDataTable.Rows[characterLevel - 1].Cells[6].Value.ToString()) + ", " + getModDamage(currentWeapon.range, currentWeapon.damage.Substring(0, currentWeapon.damage.IndexOf('/'))));
                    else charSheetBox.Items.Add(weaponName + ": " + getModAttackThrow(currentWeapon, throwDataTable.Rows[characterLevel - 1].Cells[6].Value.ToString()) + ", " + getModDamage(currentWeapon.range, currentWeapon.damage));
                }
            }

        }

        //Check if weapon is melee or ranged
        //If melee, subtract Str mod
        //If ranged, subtract Dex mod
        //Return in the format X+
        private string getModAttackThrow(weapon armament, string attackThrow)
        {
            int attack = 0;
            string returner = "";
            attack = System.Convert.ToInt32(attackThrow.Substring(0, attackThrow.IndexOf("+")));
            if (armament.range == "Melee")
            {
                attack = attack - System.Convert.ToInt32(strMod.Text);
                returner = System.Convert.ToString(attack) + "+" + " melee";
            }
            else if ((armament.range == "Ranged") || (armament.range == "Thrown"))
            {
                attack = attack - System.Convert.ToInt32(dexMod.Text);
                returner = System.Convert.ToString(attack) + "+" + " ranged";
            }
            else if (armament.range.Contains(","))
            {
                returner = getModAttackThrow(new weapon(armament.name, armament.cost, armament.damage, armament.range.Substring(0, armament.range.IndexOf(","))), attackThrow);
                returner = returner + ", " + getModAttackThrow(new weapon(armament.name, armament.cost, armament.damage, armament.range.Substring(armament.range.IndexOf(",") + 1)), attackThrow);
            }
            return returner;
        }

        private string getModDamage(string range, string baseDamage)
        {
            //If range = Melee add Str mod to damage
            //Base damage will be a string in the form of 1dX, basically, so what I want to do is add to it my str mod, and that's pretty much it for now
            if (range == "Melee")
            {
                if (System.Convert.ToInt32(strMod.Text) == 0)
                    return baseDamage;
                else
                    return baseDamage + strMod.Text;
            }
            //Melee or ranged means it needs damage for both.
            if (range.Contains(","))
            {
                return (getModDamage("Melee", baseDamage) + " melee, " + getModDamage("Ranged", baseDamage) + " ranged");
            }
            //If not, return base damage
            else
                return baseDamage;
        }

        private int getAC()
        {
            //Dex Mod + Armor AC + Shield
            //That reminds me, I need to make it possible to equip a shield
            //To the update weapon equip boxes!
            //You can equip shields now!  Back to calculating AC.

            int returner = 0;
            returner = returner + findArmor(armorEquipBox.Text).ac + System.Convert.ToInt32(dexMod.Text);
            if (weaponEquipBox2.Text == "Shield") returner++;

            return returner;
        }

        private int rollHitPoints()
        {
            int hp = 0;
            //int hp = generateNumberInt(classGridView.Rows[characterLevel - 1].Cells[3].Value.ToString());
            //HD covers their base HD
            string hd = classGridView.Rows[characterLevel - 1].Cells[3].Value.ToString();
            string hdMod = "";
            if (characterLevel < 10)
            {
                hp = rollHitDice(hd, characterLevel);
            }
            else
            {
                //Find the number being added by default:  that's everything after the + sign
                hdMod = hd.Substring(hd.IndexOf('+'));
                hd = hd.Substring(0, hd.IndexOf('+'));
                hp = rollHitDice(hd, 9);
                //Add it.
                hp += System.Convert.ToInt32(hdMod);
                //Done!
            }
            return hp;
        }

        private int rollHitDice(string hd, int num)
        {
            int hp = 0, temp = 0;
            //I need to find the sides of the dice.  Since this level is less than 10, it's everything after the d.
            string dice = hd.Substring(hd.IndexOf('d') + 1);
            //Level less than 10:  For each level, roll the die, then add/subtract con mod, then replace with 1 if less than 1
            //Optionally: Max HP at first level
            if (maxHPFirstBox.Checked)
            {
                temp = System.Convert.ToInt32(dice);
                temp = temp + System.Convert.ToInt32(conMod.Text);
                hp += temp;
                for (int i = 1; i < num; i++)
                {
                    temp = dieRoller("1d" + dice);
                    temp = temp + System.Convert.ToInt32(conMod.Text);
                    if (temp < 1) temp = 1;
                    hp += temp;
                }
            }
            else
            {
                for (int i = 0; i < num; i++)
                {
                    temp = dieRoller("1d" + dice);
                    temp = temp + System.Convert.ToInt32(conMod.Text);
                    if (temp < 1) temp = 1;
                    hp += temp;
                }
            }
            return hp;
        }

        private string getHitDice()
        {
            string hd = classGridView.Rows[characterLevel - 1].Cells[3].Value.ToString();
            string modifier = "";
            int mod = 0;
            //Level is 10 or greater in this case
            if (hd.Contains('+'))
            {
                modifier = hd.Substring(hd.IndexOf('+'));
                mod = System.Convert.ToInt32(modifier);
                mod += System.Convert.ToInt32(conMod.Text) * 9;
                //9d4+6
                hd = hd.Substring(0, hd.IndexOf('+'));
                if (mod < 0)
                    hd = hd + mod;
                else if (mod > 0)
                    hd = hd + "+" + mod;
            }
            //Level is 9 or less in this case
            else
            {
                mod = System.Convert.ToInt32(conMod.Text) * characterLevel;
                if (mod < 0)
                    hd = hd + mod;
                else if (mod > 0)
                    hd = hd + "+" + mod;
            }
            return hd;
        }

        private string getMovementSpeed()
        {
            return "120'";
        }

        private void writeProficiencies()
        {
            for (int i = 0; i < takenBox.Items.Count; i++)
            {
                charSheetBox.Items.Add(takenBox.Items[i]);
            }
        }

        public static double generateNumberDouble(string parser)
        {
            //rand = new Random(Guid.NewGuid().GetHashCode());
            int counter = 0;
            int returner = 0;
            if (parser.Contains('*'))
            {
                //Check whether it's XdY*AdB or XdY*Z
                if (parser.Substring(parser.IndexOf('*') + 1).Contains('d'))
                {

                    //XdY * AdB means roll XdY, then roll AdB a number of times equal to the result
                    //Counter now has the result of XdY
                    counter = dieRoller(parser.Substring(0, parser.IndexOf('*')));
                    for (int i = 0; i < counter; i++)
                    {
                        //Rolls the second set of dice each time through the for loop
                        returner += dieRoller(parser.Substring(parser.IndexOf('*') + 1));
                    }
                }
                else
                {
                    //XdY*Z case
                    returner += dieRoller(parser.Substring(0, parser.IndexOf('*')));
                    //After rolling dice, multiply by the value
                    returner = returner * System.Convert.ToInt32(parser.Substring(parser.IndexOf('*') + 1));
                }
            }
            else if (parser.Contains('+'))
            {
                //Does not contain a multiplier; this code covers XdY+Z
                returner += dieRoller(parser.Substring(0, parser.IndexOf('+')));
                returner += System.Convert.ToInt32(parser.Substring(parser.IndexOf('+') + 1));

            }
            else if (parser.Contains('-'))
            {
                //Does not contain a multiplier; this code covers XdY-Z
                returner += dieRoller(parser.Substring(0, parser.IndexOf('-')));
                returner -= System.Convert.ToInt32(parser.Substring(parser.IndexOf('-') + 1));
            }
            else
            //This code covers XdY
            {
                returner += dieRoller(parser);
            }
            return (double)returner;
        }


        public static int generateNumberInt(string parser)
        {
            return (int)generateNumberDouble(parser);
        }

        //Rolls XdY.  String must be in the format XdY; no plus signs or times signs or anything.
        public static int dieRoller(string parser)
        {
            if (parser == "")
            {
                return -1;
            }
            int returner = 0;
            int temp = 0;
            for (int i = 0; i < System.Convert.ToInt32(parser.Substring(0, parser.IndexOf('d'))); i++)
            {
                //Roll this many dice
                temp = System.Convert.ToInt32(parser.Substring(parser.IndexOf('d') + 1));
                returner += rand.Next(1, (temp + 1));
                //Die from one to x
            }
            return returner;
        }

        private void templateButton_Click(object sender, EventArgs e)
        {
            //Remove gear page, add template page.
            tabControl1.TabPages.Remove(gearPage);
            tabControl1.TabPages.Insert(tabControl1.TabPages.IndexOf(equipPage), templatePage);

            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(templatePage);
        }

        private void shopButton_Click(object sender, EventArgs e)
        {

            //Remove gear page, add shop
            tabControl1.TabPages.Remove(gearPage);
            tabControl1.TabPages.Insert(tabControl1.TabPages.IndexOf(equipPage), shopPage);
            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(shopPage);
        }

        private void gearSelectButton_Click(object sender, EventArgs e)
        {

            //Remove template, add gear
            tabControl1.TabPages.Remove(templatePage);
            tabControl1.TabPages.Insert(tabControl1.TabPages.IndexOf(equipPage), gearPage);
            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(gearPage);
        }

        private void templateButton2_Click(object sender, EventArgs e)
        {

            //Remove shop, add template
            tabControl1.TabPages.Remove(shopPage);
            tabControl1.TabPages.Insert(tabControl1.TabPages.IndexOf(equipPage), templatePage);

            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(templatePage);
        }

        private void gpRollButton_Click(object sender, EventArgs e)
        {
            gpBox.Text = System.Convert.ToString((rand.Next(1, 7) + rand.Next(1, 7) + rand.Next(1, 7)) * 10);
        }

        private void equipButton_click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(equipPage);
        }


        private void importWeapons()
        {
            
            string line = "";
            weapon temp = new weapon();

            System.IO.StreamReader reader = new System.IO.StreamReader(Application.StartupPath + "/Resources/Items/Weapons.txt");
            line = charClass.read(reader);

            while (!reader.EndOfStream)
            {
                //Name, Cost, Damage, Range
                temp.name = line;
                line = charClass.read(reader);
                temp.cost = System.Convert.ToInt32(line.Substring(0, line.IndexOf("gp")));
                line = charClass.read(reader);
                temp.damage = line;
                line = charClass.read(reader);
                temp.range = line;
                weapons.Add(temp);
                temp = new weapon();
                line = charClass.read(reader);
            }

            for (int i = 0; i < weapons.Count; i++)
            {
                weaponListBox.Items.Add(weapons[i].name);
            }

        }

        private void importArmors()
        {
            string line = "";
            System.IO.StreamReader reader = new System.IO.StreamReader(Application.StartupPath + "/Resources/Items/Armor.txt");
            armor temp = new armor();

            line = charClass.read(reader);

            while (!reader.EndOfStream)
            {
                //Name, Cost, AC
                temp.name = line;
                line = charClass.read(reader);
                temp.cost = System.Convert.ToInt32(line.Substring(0, line.IndexOf("gp")));
                line = charClass.read(reader);
                temp.ac = System.Convert.ToInt32(line);
                armors.Add(temp);
                temp = new armor();
                line = charClass.read(reader);
            }

            for (int i = 0; i < armors.Count; i++)
            {
                armorListBox.Items.Add(armors[i].name);
            }

        }

        private void importItems()
        {
            string line = "";
            System.IO.StreamReader reader = new System.IO.StreamReader(Application.StartupPath + "/Resources/Items/Items.txt");
            item temp = new item();

            line = charClass.read(reader);

            while (!reader.EndOfStream)
            {
                //Name, Cost
                temp.name = line;
                line = charClass.read(reader);
                temp.cost = line;
                items.Add(temp);
                temp = new item();
                line = charClass.read(reader);
            }

            for (int i = 0; i < items.Count; i++)
            {
                itemListBox.Items.Add(items[i].name);
            }

        }

        private void itemBoxSelecterAdder(ListBox selectedFrom, ListBox addedTo, string itemType)
        {
            double gp = System.Convert.ToDouble(gpBox.Text);
            double cost = 0;
            if (selectedFrom.SelectedIndex > -1)
            {
                addedTo.Items.Add(selectedFrom.GetItemText(selectedFrom.SelectedItem));
                //for (int i = 0; i < weapons.Count; i++)
                //{
                //if (weapons[i].name == weaponListBox.GetItemText(weaponListBox.SelectedItem))
                //{
                //   cost = weapons[i].cost;
                //}
                //}

                if (itemType == "weapon")
                    cost = findWeapon(selectedFrom.GetItemText(selectedFrom.SelectedItem)).cost;
                else if (itemType == "armor")
                    cost = findArmor(selectedFrom.GetItemText(selectedFrom.SelectedItem)).cost;
                else if (itemType == "item")
                    cost = findItem(selectedFrom.GetItemText(selectedFrom.SelectedItem)).getCost();

                gp = gp - cost;
                gpBox.Text = System.Convert.ToString(gp);
            }
        }

        private void selectWeaponButton_Click(object sender, EventArgs e)
        {
            itemBoxSelecterAdder(weaponListBox, weaponOwnedBox, "weapon");
        }

        private void updateEquipPage()
        {
            //Update weapon boxes
            equipBoxUpdater(weaponOwnedBox, weaponEquipBox);
            equipBoxUpdater(weaponOwnedBox, weaponEquipBox2);
            //Update armor box
            equipBoxUpdater(armorOwnedBox, armorEquipBox);

            //Special case: Shield
            //Can you DW shields in ACKS?  Hm.  Signs point to no.  That's for the AC modifier, though.
            if (armorOwnedBox.Items.Contains("Shield"))
            {
                if (!weaponEquipBox2.Items.Contains("Shield"))
                {
                    //Add shield
                    //In order to enforce the no dual wielding shields, they only show up in the off hand.
                    //Note to self:  Do not include shield as a weapon when outputting.
                    weaponEquipBox2.Items.Add("Shield");
                }
            }

            //Remove shield from the weapon box if it's not in the armor owned box anymore
            if (!armorOwnedBox.Items.Contains("Shield") && weaponEquipBox2.Items.Contains("Shield"))
                weaponEquipBox2.Items.Remove("Shield");



            if (armorEquipBox.Items.Contains("Shield")) armorEquipBox.Items.Remove("Shield");
            //armorEquipBox.Items.RemoveAt(findIndex("Shield", armorEquipBox));
        }

        private int findIndex(string test, ComboBox source)
        {
            int returner = -1;
            for (int i = 0; i < source.Items.Count; i++)
            {
                if (source.Items[i].ToString() == test)
                    returner = i;
            }
            return returner;
        }

        private void equipBoxUpdater(ListBox ownedBox, ComboBox equipBox)
        {
            for (int i = 0; i < ownedBox.Items.Count; i++)
            {
                if (!equipBox.Items.Contains(ownedBox.Items[i]))
                {
                    equipBox.Items.Add(ownedBox.Items[i]);
                    //equipBox2.Items.Add(ownedBox.Items[i]);
                }
            }

            //What am I actually trying to do here?
            // - If weaponEquipBox has an element that is not in weaponOwnedBox, remove it from weaponOwnedBox
            // - Loop through the count of weaponEquipBox; compare each element to weaponOwnedBox

            for (int i = 0; i < equipBox.Items.Count; i++)
            {
                if (!ownedBox.Items.Contains(equipBox.Items[i]))
                {
                    equipBox.Items.Remove(equipBox.Items[i]);
                    //weaponEquipBox2.Items.Remove(weaponEquipBox2.Items[i]);
                }
            }
        }

        private void weaponEquipBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string damage = "";
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i].name == weaponEquipBox.Items[weaponEquipBox.SelectedIndex].ToString())
                    damage = weapons[i].damage;
            }
            if (damage == "1d10")
            {
                weaponEquipBox2.SelectedIndex = weaponEquipBox.SelectedIndex;
                weaponEquipBox2.Enabled = false;
            }
            else
            {
                weaponEquipBox2.Enabled = true;
            }

        }


        private weapon findWeapon(string name)
        {
            int finder = -1;
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i].name == name)
                    finder = i;
            }

            if (finder > -1) return weapons[finder];
            else return new weapon();
        }

        private armor findArmor(string name)
        {
            int finder = -1;
            for (int i = 0; i < armors.Count; i++)
            {
                if (armors[i].name == name)
                    finder = i;
            }

            if (finder > -1) return armors[finder];
            else return new armor();
        }

        private item findItem(string name)
        {
            int finder = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].name == name)
                    finder = i;
            }

            if (finder > -1) return items[finder];
            else return new item();
        }

        private void unselecterItemHelper(ListBox removeFrom, string type)
        {
            double gp = System.Convert.ToDouble(gpBox.Text);
            double cost = 0;
            if (removeFrom.SelectedIndex > -1)
            {
                if (type == "weapon")
                    cost = findWeapon(removeFrom.GetItemText(removeFrom.SelectedItem)).cost;
                else if (type == "armor")
                    cost = findArmor(removeFrom.GetItemText(removeFrom.SelectedItem)).cost;
                else if (type == "item")
                    cost = findItem(removeFrom.GetItemText(removeFrom.SelectedItem)).getCost();
                removeFrom.Items.Remove(removeFrom.SelectedItem);
                //for (int i = 0; i < weapons.Count; i++)
                //{
                //if (weapons[i].name == weaponListBox.GetItemText(weaponListBox.SelectedItem))
                //{
                //   cost = weapons[i].cost;
                //}
                //}

                gp = gp + cost;
                gpBox.Text = System.Convert.ToString(gp);
            }
        }

        private void unselectWeaponBox_Click(object sender, EventArgs e)
        {
            /*int gp = System.Convert.ToInt32(gpBox.Text);
            int cost = 0;
            if (weaponOwnedBox.SelectedIndex > -1)
            {
                cost = findWeapon(weaponOwnedBox.GetItemText(weaponListBox.SelectedItem)).cost;
                weaponOwnedBox.Items.Remove(weaponOwnedBox.SelectedItem);
                //for (int i = 0; i < weapons.Count; i++)
                //{
                //if (weapons[i].name == weaponListBox.GetItemText(weaponListBox.SelectedItem))
                //{
                //   cost = weapons[i].cost;
                //}
                //}

                gp = gp + cost;
                gpBox.Text = System.Convert.ToString(gp);
            }*/
            unselecterItemHelper(weaponOwnedBox, "weapon");
        }

        private void selectArmorButton_Click(object sender, EventArgs e)
        {
            itemBoxSelecterAdder(armorListBox, armorOwnedBox, "armor");
        }

        private void selectItemButton_Click(object sender, EventArgs e)
        {
            itemBoxSelecterAdder(itemListBox, itemOwnedBox, "item");
        }

        private void unselectArmorButton_Click(object sender, EventArgs e)
        {
            unselecterItemHelper(armorOwnedBox, "armor");
        }

        private void unselectItemButton_Click(object sender, EventArgs e)
        {
            unselecterItemHelper(itemOwnedBox, "item");
        }

        private void weaponListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentItem = "";
            if (weaponListBox.SelectedIndex > -1)
                currentItem = (string)weaponListBox.Items[weaponListBox.SelectedIndex];
            else currentItem = "";

            int numItems = itemDescriptionBox.Items.Count;
            for (int i = 0; i < numItems; i++)
            {
                itemDescriptionBox.Items.RemoveAt(0);
            }

            ArrayList description = activeClass.getWeaponDescription(currentItem);
            itemDescriptionBox.Items.Add("Damage: " + findWeapon(currentItem).damage);
            itemDescriptionBox.Items.Add("Cost: " + findWeapon(currentItem).cost + " gp");

            for (int i = 0; i < description.Count; i++)
            {
                itemDescriptionBox.Items.Add(description[i]);
            }
        }

        private void weaponOwnedBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentItem = "";
            if (weaponOwnedBox.SelectedIndex > -1)
                currentItem = (string)weaponOwnedBox.Items[weaponOwnedBox.SelectedIndex];
            else currentItem = "";

            int numItems = itemDescriptionBox.Items.Count;
            for (int i = 0; i < numItems; i++)
            {
                itemDescriptionBox.Items.RemoveAt(0);
            }

            if (currentItem != "")
            {
                ArrayList description = activeClass.getWeaponDescription(currentItem);
                itemDescriptionBox.Items.Add("Damage: " + findWeapon(currentItem).damage);
                itemDescriptionBox.Items.Add("Cost: " + findWeapon(currentItem).cost + " gp");

                for (int i = 0; i < description.Count; i++)
                {
                    itemDescriptionBox.Items.Add(description[i]);
                }
            }
        }

        private void itemListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentItem = "";
            ArrayList description = new ArrayList();
            if (itemListBox.SelectedIndex > -1)
                currentItem = (string)itemListBox.Items[itemListBox.SelectedIndex];
            else currentItem = "";

            int numItems = itemDescriptionBox.Items.Count;
            for (int i = 0; i < numItems; i++)
            {
                itemDescriptionBox.Items.RemoveAt(0);
            }

            if (!currentItem.Contains("("))
            {
                description = activeClass.getItemDescription(currentItem);
            }
            else
            {
                description = activeClass.getItemDescription(currentItem.Substring(0, currentItem.IndexOf("(")-1));
            }
            itemDescriptionBox.Items.Add("Cost: " + findItem(currentItem).cost);

            for (int i = 0; i < description.Count; i++)
            {
                itemDescriptionBox.Items.Add(description[i]);
            }
        }

        private void itemOwnedBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentItem = "";
            ArrayList description = new ArrayList();
            if (itemOwnedBox.SelectedIndex > -1)
                currentItem = (string)itemOwnedBox.Items[itemOwnedBox.SelectedIndex];
            else currentItem = "";

            int numItems = itemDescriptionBox.Items.Count;
            for (int i = 0; i < numItems; i++)
            {
                itemDescriptionBox.Items.RemoveAt(0);
            }

            if (!currentItem.Contains("("))
            {
                description = activeClass.getItemDescription(currentItem);
            }
            else
            {
                string temp = currentItem.Substring(0, currentItem.IndexOf("("));
                description = activeClass.getItemDescription(currentItem.Substring(0, currentItem.IndexOf("(")));
            }
            itemDescriptionBox.Items.Add("Cost: " + findItem(currentItem).cost);

            for (int i = 0; i < description.Count; i++)
            {
                itemDescriptionBox.Items.Add(description[i]);
            }
        }

        private void armorListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentItem = "";
            if (armorListBox.SelectedIndex > -1)
                currentItem = (string)armorListBox.Items[armorListBox.SelectedIndex];
            else currentItem = "";

            int numItems = itemDescriptionBox.Items.Count;
            for (int i = 0; i < numItems; i++)
            {
                itemDescriptionBox.Items.RemoveAt(0);
            }

            ArrayList description = activeClass.getArmorDescription(currentItem);
            itemDescriptionBox.Items.Add("AC: " + findArmor(currentItem).ac);
            itemDescriptionBox.Items.Add("Cost: " + findArmor(currentItem).cost + " gp");

            for (int i = 0; i < description.Count; i++)
            {
                itemDescriptionBox.Items.Add(description[i]);
            }
        }

        private void armorOwnedBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentItem = "";
            if (armorOwnedBox.SelectedIndex > -1)
                currentItem = (string)armorOwnedBox.Items[armorOwnedBox.SelectedIndex];
            else currentItem = "";

            int numItems = itemDescriptionBox.Items.Count;
            for (int i = 0; i < numItems; i++)
            {
                itemDescriptionBox.Items.RemoveAt(0);
            }

            ArrayList description = activeClass.getArmorDescription(currentItem);
            itemDescriptionBox.Items.Add("AC: " + findArmor(currentItem).ac);
            itemDescriptionBox.Items.Add("Cost: " + findArmor(currentItem).cost + " gp");

            for (int i = 0; i < description.Count; i++)
            {
                itemDescriptionBox.Items.Add(description[i]);
            }
        }




    }
}
