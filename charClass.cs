using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace ACKS_CB
{

    //An instance of this class should be declared for each character class available in the builder.
    //Thus, not a static class; the constructor needs to be passed the class name so it can get the reader set up.
    class charClass
    {
        private int hd;
        private int fighting;
        private int arcane;
        private int arcaneDelayed;
        private int divine;
        private int divineDelayed;
        private int thievery;
        private int psionic;
        private int psionicDelayed;
        private int elfValue;
        private string primeReq;
        private string requirements;
        private int maxLevel;
        private ArrayList levelTitles;
        private ArrayList xpValues;
        private ArrayList specials;
        private ArrayList proficiencies;
        private ArrayList thiefSkills;
        private string saveProgression;
        private string weapons;
        private string armor;
        private string styles;
        public bool blessed = false;
        public bool dwarf = false;
        public bool elf = false;

        private string line;
        private System.IO.StreamReader classReader;

        //TODO:  Pass the name of the class to the constructor so it can set up the stream reader.
        //Only the active class will be instantiated as a charClass.
        public charClass()
        {
            hd = 0;
            fighting = 0;
            arcane = 0;
            divine = 0;
            thievery = 0;
            psionic = 0;
            arcaneDelayed = 0;
            divineDelayed = 0;
            psionicDelayed = 0;
            elfValue = 0;
            line = "";
            primeReq = "";
            requirements = "";
            maxLevel = 0;
            levelTitles = new ArrayList();
            xpValues = new ArrayList();
            specials = new ArrayList();
            proficiencies = new ArrayList();
            thiefSkills = new ArrayList();
            saveProgression = "";
            weapons = "";
            armor = "";
            styles = "";
        }

        public charClass(string className)
        {
            hd = 0;
            fighting = 0;
            arcane = 0;
            divine = 0;
            thievery = 0;
            psionic = 0;
            arcaneDelayed = 0;
            divineDelayed = 0;
            psionicDelayed = 0;
            elfValue = 0;
            line = "";
            primeReq = "";
            requirements = "";
            maxLevel = 0;
            levelTitles = new ArrayList();
            xpValues = new ArrayList();
            specials = new ArrayList();
            proficiencies = new ArrayList();
            thiefSkills = new ArrayList();
            classReader = new System.IO.StreamReader(Application.StartupPath + "/Resources/Classes/" + className + ".txt");
            saveProgression = "";
            weapons = "";
            armor = "";
            styles = "";
            importClass();
        }

        public string getPrimeReq()
        {
            return primeReq;
        }

        public string getRequirements()
        {
            return requirements;
        }

        public int getMaxLevel()
        {
            return maxLevel;
        }

        public int getHD()
        {
            return hd;
        }

        public int getFighting()
        {
            return fighting;
        }

        public int getArcane()
        {
            return arcane;
        }

        public int getArcaneDelay()
        {
            return arcaneDelayed;
        }

        public int getDivine()
        {
            return divine;
        }

        public int getDivineDelay()
        {
            return divineDelayed;
        }

        public int getThievery()
        {
            return thievery;
        }

        public int getPsionic()
        {
            return psionic;
        }

        public int getPsionicDelay()
        {
            return psionicDelayed;
        }

        public int getElfValue()
        {
            return elfValue;
        }

        public ArrayList getLevelTitles()
        {
            return levelTitles;
        }

        public ArrayList getXPValues()
        {
            return xpValues;
        }

        public ArrayList getSpecials()
        {
            return specials;
        }

        public ArrayList getProfs()
        {
            return proficiencies;
        }

        public ArrayList getThiefSkills()
        {
            return thiefSkills;
        }

        public ArrayList getSpecialsShort()
        {
            ArrayList temp = new ArrayList();
            string storage = "";

            for (int i = 0; i < specials.Count; i++)
            {
                storage = (string)specials[i];
                if (storage == "")
                {
                    temp.Add(storage);
                }
                else
                {
                    storage = storage.Substring(0, storage.IndexOf(":"));
                    temp.Add(storage);
                }
            }

            return temp;
        }

        public ArrayList getSpecialsLong()
        {
            ArrayList temp = new ArrayList();
            string storage = "";

            for (int i = 0; i < specials.Count; i++)
            {
                storage = (string)specials[i];
                if (storage == "")
                {
                    temp.Add(storage);
                }
                else
                {
                    storage = storage.Substring(storage.IndexOf(":")+1);
                    temp.Add(storage);
                }
            }

            return temp;
        }

        public string getSaveProgression()
        {
            return saveProgression;
        }

        public string getWeapons()
        {
            return weapons;
        }

        public string getArmor()
        {
            return armor;
        }

        public string getStyles()
        {
            return styles;
        }

        //Reads lines until it gets to one that isn't commented out.
        //Use as primary reader instead of calling ReadLine directly; thus, I never have to worry about commented lines again
        private string read()
        {
            string temp;
            if ((classReader == null) || classReader.EndOfStream) return "*****";
            temp = classReader.ReadLine();
            if (temp.Length < 2) return temp;

            while (temp.Substring(0, 2) == "//")
            {
                temp = classReader.ReadLine();
                if (temp.Length < 2) return temp;
            }
            return temp;
        }

        public static string read(System.IO.StreamReader reader)
        {
            string temp;
            if ((reader == null) || reader.EndOfStream) return "*****";
            temp = reader.ReadLine();
            if (temp.Length < 2) return temp;

            while (temp.Substring(0, 2) == "//")
            {
                temp = reader.ReadLine();
                if (temp.Length < 2) return temp;
            }
            return temp;
        }

        private void importClass()
        {
            if (classReader == null) return;
            //Block 1 Begins
            //Prime Requisite
            primeReq = read();
            //Requirements
            requirements = read();
            //Max Level
            maxLevel = System.Convert.ToInt32(read());
            //Weapons
            weapons = read();
            //Armor
            armor = read();
            //Fighting Styles
            styles = read();
            //Block 2 Begins
            //Block 1 is known length; therefore, throw away the *'s
            read();
            //Class Values
            importClassValues();
            //Block 3 Begins: The current 'line' value is ****** etc, so a new read will give us the first line of block 3
            //Level Titles
            importLevelTitles();
            //Block 4: As Block 3
            //XP Values
            importXPValues();
            //Attack and Saving Throws
            //May not be necessary!  Ignoring for now, commented out in file.
            //Special Abilities
            importSpecials();
            //Proficiency List
            importProfs();

            //Thief Skills:  Only happens with classes that have Thievery 1 or higher
            if (thievery > 0) importThiefSkills();

            if (saveProgression.ToUpper() == "BLESSED")
            {
                blessed = true;
                determineSaveProgression();
            }
            else if (saveProgression.ToUpper() == "DWARF")
            {
                dwarf = true;
                determineSaveProgression();
            }
            else if (saveProgression.ToUpper() == "ELF")
            {
                elf = true;
                determineSaveProgression();
            }
            else determineSaveProgression();
            classReader.Close();
        }

        private void importClassValues()
        {
            //HD, Fighting, Arcane, Divine, Thievery, Psionic
            line = read();
            int index;
            index = 0;
            while (!line.Contains("****"))
            {
                if (line.Contains("Hit Dice")){
                    index = line.IndexOf("Hit Dice") + 9;
                    hd = System.Convert.ToInt32(line.Substring(index));
                }
                else if (line.Contains("Fighting")){
                    index = line.IndexOf("Fighting") + 9;
                    fighting = System.Convert.ToInt32(line.Substring(index));
                }
                else if (line.Contains("Arcane")){
                    index = line.IndexOf("Arcane") + 7;
                    //arcane = System.Convert.ToInt32(line.Substring(index));
                    getSpellcastingDelayValue(0, line.Substring(index));
                }
                else if (line.Contains("Divine")){
                    index = line.IndexOf("Divine") + 7;
                    //divine = System.Convert.ToInt32(line.Substring(index));
                    getSpellcastingDelayValue(1, line.Substring(index));
                }
                else if (line.Contains("Thievery")){
                    index = line.IndexOf("Thievery") + 9;
                    thievery = System.Convert.ToInt32(line.Substring(index));
                }
                else if (line.Contains("Psionic")){
                    index = line.IndexOf("Psionic") + 8;
                    //psionic = System.Convert.ToInt32(line.Substring(index));
                    getSpellcastingDelayValue(2, line.Substring(index));
                }
                else if (line.Contains("Save Progression"))
                {
                    index = line.IndexOf(": ") + 2;
                    saveProgression = line.Substring(index);
                }
                else if (line.Contains("Elf"))
                {
                    index = line.IndexOf("Elf") + 4;
                    elfValue = System.Convert.ToInt32(line.Substring(index));
                }
                line = read();
            }
        }

        private void getSpellcastingDelayValue(int type, string tester)
        {
            //I will be passed a string for the type (arcane, divine, psionic)
            //And a string in the form X or XA, where X is a number
            //Changing type to an int: 0, 1, 2 for arcane, divine, psionic
            switch (type)
            {
                    //Case 0: Arcane
                case 0:
                    if (tester.ToUpper().Contains("A"))
                    {
                        arcane = System.Convert.ToInt32(tester.Substring(0, tester.Length - 1));
                        if (arcane + elfValue == 1)
                        {
                            arcaneDelayed = 7;
                        }
                        else if (arcane + elfValue == 2)
                        {
                            arcaneDelayed = 5;
                        }
                        else if (arcane + elfValue == 3)
                        {
                            arcaneDelayed = 3;
                        }
                    }
                    else arcane = System.Convert.ToInt32(tester);
                    break;
                    //Case 1: Divine
                case 1:
                    if (tester.ToUpper().Contains("A"))
                    {
                        divine = System.Convert.ToInt32(tester.Substring(0, tester.Length - 1));
                        if (divine == 1)
                        {
                            divineDelayed = 5;
                        }
                    }
                    else divine = System.Convert.ToInt32(tester);
                    break;
                    //Case 2: Psionic
                case 2:
                    if (tester.ToUpper().Contains("A"))
                    {
                        psionic = System.Convert.ToInt32(tester.Substring(0, tester.Length - 1));
                        if (psionic == 1)
                        {
                            psionicDelayed = 7;
                        }
                        else if (psionic == 2)
                        {
                            psionicDelayed = 3;
                        }
                    }
                    else psionic = System.Convert.ToInt32(tester);
                    break;
            }
        }

        private void importLevelTitles()
        {
            line = read();
            while (!line.Contains("****"))
            {
                levelTitles.Add(line);
                line = read();
            }
        }

        private void importXPValues()
        {
            line = read();
            while (!line.Contains("****"))
            {
                xpValues.Add(line);
                line = read();
            }
        }

        private void importSpecials()
        {
            line = read();
            while (!line.Contains("****"))
            {
                specials.Add(line);
                line = read();
            }
            padSpecials();
        }

        private void padSpecials()
        {
            ArrayList temp = new ArrayList();
            for (int i = 0; i < maxLevel; i++)
            {
                temp.Add("");
            }
            String placeholder = "";
            int counter = 0;
            for (int i = 0; i < specials.Count; i++)
            {
                placeholder = (string)specials[i];
                counter = System.Convert.ToInt32(placeholder.Substring(0, placeholder.IndexOf(":"))) - 1;
                placeholder = placeholder.Substring(placeholder.IndexOf(":") + 1);
                temp[counter] = placeholder;
            }
            specials = temp;

        }

        private void importProfs()
        {
            line = read();
            while (!line.Contains("****"))
            {
                proficiencies.Add(line);
                line = read();
            }
        }

        private void importThiefSkills()
        {
            line = read();
            while (!line.Contains("****"))
            {
                thiefSkills.Add(line);
                line = read();
            }
        }

        private void determineSaveProgression()
        {
            //If Arcane >= all others, mage
            if ((arcane >= fighting) && (arcane >= thievery) && (arcane >= divine) && (arcane >= psionic))
                saveProgression = "Mage";
            //If Divine >= all others, cleric
            else if ((divine >= arcane) && (divine >= thievery) && (divine >= fighting) && (divine >= psionic))
                saveProgression = "Cleric";
            //If Psionic >= all others, thief, because I set psionic characters to have identical saving throws to thieves
            else if ((psionic >= arcane) && (psionic >= thievery) && (psionic >= fighting) && (psionic >= divine))
                saveProgression = "Thief";
            //If Thievery >= all others, thief
            else if ((thievery >= arcane) && (thievery >= divine) && (thievery >= fighting) && (thievery >= psionic))
                saveProgression = "Thief";
            //If Fighting >= all others, fighter
            else if ((fighting >= arcane) && (fighting >= divine) && (fighting >= thievery) && (fighting >= psionic))
                saveProgression = "Fighter";
            else saveProgression = "Error";
        }

        private List<List<string>> convertArcane(double conversion)
        {
            //As mage of level * Conversion
            List<List<string>> mage = arcane4();
            List<List<string>> spells = new List<List<string>>();

            for (int i = 0; i < mage.Count; i++)
            {
                spells.Add(new List<string>());
                for (int j = 0; j < mage[i].Count; j++)
                {
                    spells[i].Add("-");
                }
            }

            string temp;

            //I = SPELL LEVEL
            //J = CHARACTER LEVEL
            double level = 0;
            int effectiveLevel = 0;
            for (int i = 0; i < mage.Count; i++)
            {
                for (int j = 0; j < mage[i].Count; j++)
                {
                    level = (System.Convert.ToDouble(j+1)) * conversion;
                    if (conversion < 0.5)
                    {
                        effectiveLevel = System.Convert.ToInt32(Math.Truncate(level)) - 1;
                    }
                    else
                    {
                        effectiveLevel = System.Convert.ToInt32(Math.Round(level)) - 1;
                    }
                    
                    if (effectiveLevel < 0)
                    {
                        temp = "-";
                    }
                    else
                    {
                        temp = System.Convert.ToString(mage[i][effectiveLevel]);
                    }

                    spells[i][j] = temp;
                }
            }
            return spells;
        }

        private List<List<string>> convertDivine(double conversion)
        {
            //As mage of level * Conversion
            List<List<string>> cleric = divine2();
            List<List<string>> spells = new List<List<string>>();

            for (int i = 0; i < cleric.Count; i++)
            {
                spells.Add(new List<string>());
                for (int j = 0; j < cleric[i].Count; j++)
                {
                    spells[i].Add("-");
                }
            }

            string temp;

            //I = SPELL LEVEL
            //J = CHARACTER LEVEL
            double level = 0;
            int effectiveLevel = 0;
            for (int i = 0; i < cleric.Count; i++)
            {
                for (int j = 0; j < cleric[i].Count; j++)
                {
                    level = (System.Convert.ToDouble(j + 1)) * conversion;
                    if (conversion < 0.5)
                    {
                        effectiveLevel = System.Convert.ToInt32(Math.Truncate(level)) - 1;
                    }
                    else
                    {
                        effectiveLevel = System.Convert.ToInt32(Math.Round(level)) - 1;
                    }

                    if (effectiveLevel < 0)
                    {
                        temp = "-";
                    }
                    else
                    {
                        temp = System.Convert.ToString(cleric[i][effectiveLevel]);
                    }

                    spells[i][j] = temp;
                }
            }
            return spells;
        }

        private List<List<string>> enhancedDivine(double conversion)
        {
            List<List<string>> cleric = divine2();
            List<List<string>> spells = new List<List<string>>();

            for (int i = 0; i < cleric.Count; i++)
            {
                spells.Add(new List<string>());
                for (int j = 0; j < cleric[i].Count; j++)
                {
                    spells[i].Add("-");
                }
            }

            string temp = "";

            //I = SPELL LEVEL
            //J = CHARACTER LEVEL
            double numSpells = 0;
            for (int i = 0; i < cleric.Count; i++)
            {
                for (int j = 0; j < cleric[i].Count; j++)
                {
                    temp = cleric[i][j];
                    if (temp == "-") temp = "0";
                    numSpells = System.Convert.ToDouble(temp);
                    numSpells = numSpells * conversion;

                    temp = System.Convert.ToString(System.Convert.ToInt32(Math.Round(numSpells)));

                    if (temp == "0") temp = "-";

                    spells[i][j] = temp;
                }
            }
            return spells;
        }

        public List<List<string>> divine1()
        {
            return convertDivine(0.5);
        }

        public List<List<string>> divine2()
        {
            /*
             * 1st: 0
             * 2nd: 1
             * 3rd: 2
             * 4th: 2/1
             * 5th: 2/2
             * 6th: 2/2/1/1
             * 7th: 2/2/2/1/1
             * 8th: 3/3/2/2/1
             * 9th: 3/3/3/2/2
             * 10th: 4/4/3/3/2
             * 11th: 4/4/4/3/3
             * 12th: 5/5/4/4/3
             * 13th: 5/5/5/4/3
             * 14th: 6/5/5/5/4
             * */
            List<List<string>> spells = new List<List<string>>();

            for (int i = 0; i < 5; i++)
            {
                spells.Add(new List<string>());
            }

            //FIRST LEVEL SPELLS
            spells[0].Add("-");
            spells[0].Add("1");
            for (int i = 0; i < 5; i++)
            {
                spells[0].Add("2");
            }
            spells[0].Add("3");
            spells[0].Add("3");
            spells[0].Add("4");
            spells[0].Add("4");
            spells[0].Add("5");
            spells[0].Add("5");
            spells[0].Add("6");

            //SECOND LEVEL SPELLS
            for (int i = 0; i < 3; i++)
            {
                spells[1].Add("-");
            }
            spells[1].Add("1");
            for (int i = 0; i < 3; i++)
            {
                spells[1].Add("2");
            }
            //2 3's, 2 4's, 3 5's
            spells[1].Add("3");
            spells[1].Add("3");
            spells[1].Add("4");
            spells[1].Add("4");
            for (int i = 0; i < 3; i++)
            {
                spells[1].Add("5");
            }

            //THIRD LEVEL SPELLS
            for (int i = 0; i < 5; i++)
            {
                spells[2].Add("-");
            }
            //a 1, 2 2's, 2 3's, 2 4's, 2 5's
            spells[2].Add("1");
            spells[2].Add("2");
            spells[2].Add("2");
            spells[2].Add("3");
            spells[2].Add("3");
            spells[2].Add("4");
            spells[2].Add("4");
            spells[2].Add("5");
            spells[2].Add("5");

            //FOURTH LEVEL SPELLS
            for (int i = 0; i < 5; i++)
            {
                spells[3].Add("-");
            }
            //2 each of 1, 2, 3, 4; then a 5
            spells[3].Add("1");
            spells[3].Add("1");
            spells[3].Add("2");
            spells[3].Add("2");
            spells[3].Add("3");
            spells[3].Add("3");
            spells[3].Add("4");
            spells[3].Add("4");
            spells[3].Add("5");

            //FIFTH LEVEL SPELLS
            for (int i = 0; i < 6; i++)
            {
                spells[4].Add("-");
            }
            //2 1's, 2 2's, 3 3', and a 4
            spells[4].Add("1");
            spells[4].Add("1");
            spells[4].Add("2");
            spells[4].Add("2");
            for (int i = 0; i < 3; i++)
            {
                spells[4].Add("3");
            }
            spells[4].Add("4");

            return spells;
        }

        public List<List<string>> divine3()
        {
            return enhancedDivine(1.33333333333333333333);
        }

        public List<List<string>> divine4()
        {
            return enhancedDivine(1.5);
        }

        public List<List<string>> divine5()
        {
            return new List<List<string>>();
        }

        public List<List<string>> divine6()
        {
            return new List<List<string>>();
        }

        public List<List<string>> divine7()
        {
            return new List<List<string>>();
        }

        public List<List<string>> divine8()
        {
            return new List<List<string>>();
        }

        public List<List<string>> delayedDivine()
        {
            //METHOD TO DO
            //Start by making the list for divine 2, since all delayed acquisitions progress as a wizard normally
            //Then move the list up, deleting off the top ends, by the amount of the delay
            List<List<string>> spells = divine2();

            //Each column is a spell level
            //spells[i], 0 through 4, is the 5 spell levels
            //For each of those, I want to add a hyphen at position 0
            //I want to do this a number of times equal to the arcane delayed value

            //Do total thing number of times equal to delay
            for (int i = 0; i < divineDelayed; i++)
            {
                //Six times
                for (int j = 0; j < 5; j++)
                {
                    //Add the hyphens to each column
                    spells[j].Insert(0, "-");
                }
            }

            return spells;
        }


        public List<List<string>> arcane1()
        {
            return convertArcane(0.33333333333333);
        }

        public List<List<string>> arcane2()
        {

            return convertArcane(0.5);
        }

        public List<List<string>> arcane3()
        {
            return convertArcane(0.6666666666666667);
        }

        public List<List<string>> arcane4()
        {
            /*
             * 1st level: 1
             * 2nd level: 2
             * 3rd level: 2/1
             * 4th level: 2/2
             * 5th level: 2/2/1
             * 6th level: 2/2/2
             * 7th level: 3/2/2/1
             * 8th level: 3/3/3/2
             * 9th level: 3/3/3/2/1
             * 10th level: 3/3/3/3/2
             * 11th level: 4/3/3/3/2/1
             * 12th level: 4/4/3/3/3/2
             * 13th level: 4/4/4/3/3/2
             * 14th level: 4/4/4/4/3/3
            */
            List<List<string>> spells = new List<List<string>>();

            //FIRST LEVEL SPELLS
            for (int i = 0; i < 6; i++)
            {
                spells.Add(new List<string>());
            }
            spells[0].Add("1");
            for (int i = 0; i < 5; i++)
            {
                spells[0].Add("2");
            }
            for (int i = 0; i < 4; i++)
            {
                spells[0].Add("3");
            }
            for (int i = 0; i < 4; i++)
            {
                spells[0].Add("4");
            }

            //SECOND LEVEL SPELLS
            for (int i = 0; i < 2; i++)
            {
                spells[1].Add("-");
            }
            spells[1].Add("1");

            for (int i = 0; i < 4; i++)
            {
                spells[1].Add("2");
            }
            for (int i = 0; i < 4; i++)
            {
                spells[1].Add("3");
            }
            for (int i = 0; i < 3; i++)
            {
                spells[1].Add("4");
            }

            //THIRD LEVEL SPELLS
            for (int i = 0; i < 4; i++)
            {
                spells[2].Add("-");
            }
            spells[2].Add("1");
            for (int i = 0; i < 2; i++)
            {
                spells[2].Add("1");
            }
            for (int i = 0; i < 5; i++)
            {
                spells[2].Add("3");
            }
            for (int i = 0; i < 2; i++)
            {
                spells[2].Add("4");
            }
            //FOURTH LEVEL SPELLS
            for (int i = 0; i < 6; i++)
            {
                spells[3].Add("-");
            }
            spells[3].Add("1");
            for (int i = 0; i < 2; i++)
            {
                spells[3].Add("2");
            }
            for (int i = 0; i < 4; i++)
            {
                spells[3].Add("3");
            }
            spells[3].Add("4");
            //FIFTH LEVEL SPELLS
            for (int i = 0; i < 8; i++)
            {
                spells[4].Add("-");
            }
            spells[4].Add("1");
            for (int i = 0; i < 2; i++)
            {
                spells[4].Add("2");
            }
            for (int i = 0; i < 3; i++)
            {
                spells[4].Add("3");
            }
            //SIXTH LEVEL SPELLS
            for (int i = 0; i < 10; i++)
            {
                spells[5].Add("-");
            }
            spells[5].Add("1");
            spells[5].Add("2");
            spells[5].Add("2");
            spells[5].Add("3");

            return spells;
        }

        private List<List<string>> enhancedArcane(double conversion)
        {
            List<List<string>> mage = arcane4();
            List<List<string>> spells = new List<List<string>>();

            for (int i = 0; i < mage.Count; i++)
            {
                spells.Add(new List<string>());
                for (int j = 0; j < mage[i].Count; j++)
                {
                    spells[i].Add("-");
                }
            }

            string temp = "";

            //I = SPELL LEVEL
            //J = CHARACTER LEVEL
            double numSpells = 0;
            for (int i = 0; i < mage.Count; i++)
            {
                for (int j = 0; j < mage[i].Count; j++)
                {
                    temp = mage[i][j];
                    if (temp == "-") temp = "0";
                    numSpells = System.Convert.ToDouble(temp);
                    numSpells = numSpells * conversion;

                    temp = System.Convert.ToString(System.Convert.ToInt32(Math.Round(numSpells)));

                    if (temp == "0") temp = "-";

                    spells[i][j] = temp;
                }
            }
            return spells;
        }

        public List<List<string>> arcane5()
        {
            return enhancedArcane(1.333333);
        }

        public List<List<string>> arcane6()
        {
            return enhancedArcane(1.5);
        }

        public List<List<string>> arcane7()
        {
            return enhancedArcane(1.6666666666);
        }

        public List<List<string>> arcane8()
        {
            return enhancedArcane(2);
        }

        public List<List<string>> delayedArcane()
        {
            //METHOD TO DO
            //Start by making the list for arcane 4, since all delayed acquisitions progress as a wizard normally
            //Then move the list up, deleting off the top ends, by the amount of the delay
            List<List<string>> spells = arcane4();

            //Each column is a spell level
            //spells[i], 0 through 5, is the 6 spell levels
            //For each of those, I want to add a hyphen at position 0
            //I want to do this a number of times equal to the arcane delayed value

            //Do total thing number of times equal to arcane delayed
            for (int i = 0; i < arcaneDelayed; i++)
            {
                //Six times
                for (int j = 0; j < 6; j++)
                {
                    //Add the hyphens to each column
                    spells[j].Insert(0, "-");
                }
            }

            return spells;
        }

        //Psionic 1 is 1/3 level psionicist, smoothed; I need to put this in manually.
        public List<List<string>> psionic1()
        {
            List<List<string>> powers = new List<List<String>>();

            for (int i = 0; i < 4; i++)
            {
                powers.Add(new List<string>());
            }

            //DISCIPLINES, SCIENCES, DEVOTIONS, POWER THROW
            //These things make up the columns.
            //PSPs are calculated on the character side

            //Disciplines
            for (int i = 0; i < 5; i++)
            {
                powers[0].Add("1");
            }

            for (int i = 0; i < 9; i++)
            {
                powers[0].Add("2");
            }

            //Sciences
            powers[1].Add("0");
            powers[1].Add("0");

            for (int i = 0; i < 6; i++)
            {
                powers[1].Add("1");
            }

            for (int i = 0; i < 5; i++)
            {
                powers[1].Add("2");
            }

            powers[1].Add("3");

            //Devotions
            powers[2].Add("1");
            powers[2].Add("2");
            powers[2].Add("3");
            powers[2].Add("3");
            powers[2].Add("4");
            powers[2].Add("5");
            powers[2].Add("5");
            powers[2].Add("6");
            powers[2].Add("7");
            powers[2].Add("7");
            powers[2].Add("8");
            powers[2].Add("9");
            powers[2].Add("9");
            powers[2].Add("10");

            //Power Throw
            for (int i = 0; i < 3; i++)
            {
                powers[3].Add("10+");
            }

            for (int i = 0; i < 8; i++)
            {
                powers[3].Add("9+");
            }

            powers[3].Add("8+");
            powers[3].Add("8+");
            powers[3].Add("7+");

            return powers;
        }

        //Psionic 2 is 2/3 level psionicist, smoothed; again, manually.
        public List<List<string>> psionic2()
        {
            List<List<string>> powers = new List<List<String>>();

            for (int i = 0; i < 4; i++)
            {
                powers.Add(new List<string>());
            }

            //DISCIPLINES, SCIENCES, DEVOTIONS, POWER THROW
            //These things make up the columns.
            //PSPs are calculated elsewhere

            //Disciplines
            powers[0].Add("1");
            powers[0].Add("1");

            for (int i = 0; i < 6; i++)
            {
                powers[0].Add("2");
            }

            for (int i = 0; i < 6; i++)
            {
                powers[0].Add("3");
            }

            //Sciences
            for (int i = 1; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    powers[1].Add(i.ToString());
                }
            }
            powers[1].Add("5");
            powers[1].Add("5");

            //Devotions

            powers[2].Add("3");
            powers[2].Add("3");
            powers[2].Add("5");
            powers[2].Add("7");
            powers[2].Add("7");
            powers[2].Add("9");
            powers[2].Add("10");
            powers[2].Add("10");
            powers[2].Add("11");
            powers[2].Add("12");
            powers[2].Add("12");
            powers[2].Add("13");
            powers[2].Add("14");
            powers[2].Add("14");

            //Power Throw
            powers[3].Add("10+");
            powers[3].Add("10+");

            for (int i = 0; i < 3; i++)
            {
                powers[3].Add("9+");
            }

            powers[3].Add("8+");
            powers[3].Add("7+");
            powers[3].Add("7+");
            powers[3].Add("7+");
            powers[3].Add("6+");
            powers[3].Add("6+");
            powers[3].Add("5+");
            powers[3].Add("5+");
            powers[3].Add("5+");



            return powers;
        }

        //Psionic 3 is the standard psionicist.
        public List<List<string>> psionic3()
        {
            List<List<string>> powers = new List<List<String>>();

            for (int i = 0; i < 4; i++)
            {
                powers.Add(new List<string>());
            }

            //DISCIPLINES, SCIENCES, DEVOTIONS, POWER THROW
            //These things make up the columns.
            //PSPs are calculated on the character side
            
            //DISCIPLINES
            //Column 0
            powers[0].Add("1");
            for (int i = 0; i < 4; i++)
            {
                powers[0].Add("2");
            }

            for (int i = 0; i < 4; i++)
            {
                powers[0].Add("3");
            }

            for (int i = 0; i < 4; i++)
            {
                powers[0].Add("4");
            }

            powers[0].Add("5");

            //SCIENCES
            int temp = 1;
            //1, 1, 2, 2, 3, 3, etc
            for (int i = 0; i < 7; i++)
            {
                powers[1].Add(temp.ToString());
                powers[1].Add(temp.ToString());
                temp++;
            }

            //DEVOTIONS
            //3, then +2 per level up to level 4
            //then +1 per level afterward
            temp = 3;
            for (int i = 0; i < 4; i++)
            {
                powers[2].Add(temp.ToString());
                temp = temp + 2;
            }
            temp = 10;
            for (int i = 0; i < 10; i++)
            {
                powers[2].Add(temp.ToString());
                temp++;
            }

            //POWER THROW
            powers[3].Add("10+");
            powers[3].Add("9+");
            powers[3].Add("9+");
            powers[3].Add("8+");
            powers[3].Add("7+");
            powers[3].Add("7+");
            powers[3].Add("6+");
            powers[3].Add("5+");
            powers[3].Add("5+");
            powers[3].Add("4+");
            powers[3].Add("3+");
            powers[3].Add("3+");
            powers[3].Add("2+");
            powers[3].Add("1+");

            return powers;
        }

        //Enhanced psionic.
        public List<List<string>> psionic4()
        {
            return enhancedPsionic(1.333333333);
        }

        public List<List<string>> psionic5()
        {
            return enhancedPsionic(1.666666666666);
        }

        public List<List<string>> psionic6()
        {
            return enhancedPsionic(2);
        }

        public List<List<string>> psionic7()
        {
            return enhancedPsionic(2.333333333);
        }

        public List<List<string>> psionic8()
        {
            return enhancedPsionic(2.666666666666);
        }

        public List<List<string>> delayedPsionic()
        {
            //METHOD TO DO
            //Start by making the list for divine 2, since all delayed acquisitions progress as a wizard normally
            //Then move the list up, deleting off the top ends, by the amount of the delay
            List<List<string>> powers = psionic3();

            //Each column is a thing
            //powers[i], 0 through 3, is the 4 columns
            //For each of those, I want to add a hyphen at position 0
            //I want to do this a number of times equal to the arcane delayed value

            //Do total thing number of times equal to delay
            for (int i = 0; i < psionicDelayed; i++)
            {
                //Six times
                for (int j = 0; j < 4; j++)
                {
                    //Add the hyphens to each column
                    powers[j].Insert(0, "-");
                }
            }

            return powers;
        }

        private List<List<string>> enhancedPsionic(double conversion)
        {
            List<List<string>> psion = psionic3();
            List<List<string>> powers = new List<List<string>>();

            for (int i = 0; i < psion.Count; i++)
            {
                powers.Add(new List<string>());
                for (int j = 0; j < psion[i].Count; j++)
                {
                    powers[i].Add("-");
                }
            }

            string temp = "";

            //I = COLUMN NUMBER
            //J = CHARACTER LEVEL
            //Disciplines are not enhanced; neither is power throw.
            double numPowers = 0;
            for (int i = 1; i < psion.Count-1; i++)
            {
                for (int j = 0; j < psion[i].Count; j++)
                {
                    temp = psion[i][j];
                    if (temp == "-") temp = "0";
                    numPowers = System.Convert.ToDouble(temp);
                    numPowers = numPowers * conversion;

                    temp = System.Convert.ToString(System.Convert.ToInt32(Math.Round(numPowers)));

                    if (temp == "0") temp = "-";

                    powers[i][j] = temp;
                }
            }
            for (int i = 0; i < psion[0].Count; i++)
            {
                powers[0][i] = psion[0][i];
                powers[3][i] = psion[3][i];
            }

            return powers;
        }

        public string getPSPS(int intMod, int wisMod, int conMod, int level)
        {
            //PSPs = Int Mod + Con Mod + (Level * (10 + Wis Mod))
            int psps = intMod + conMod + ((level - psionicDelayed) * (getBasePSPs() + wisMod));
            string returner = "";

            if (psps <= 0) returner = "-";
            else returner = psps.ToString();

            return returner;
        }

        //Based on psionic value
        //Value 1 = 3, Value 2 = 7, Value 3 = 10, Value 4 = 13
        //Equal to (Value/3) * 10, rounded
        public int getBasePSPs()
        {
            int returner = 0;

            double value = (double)psionic / 3;
            value = value * 10;
            returner = System.Convert.ToInt32(value);
            return returner;
        }

        public ArrayList getDescription(string prof)
        {
            ArrayList returner = new ArrayList();
            string temp = "";
            string tester = "";
            System.IO.StreamReader profsReader = new System.IO.StreamReader(Application.StartupPath + "/Resources/Classes/Proficiencies.txt");

            if (prof.Contains("Profession")) prof = "Profession";
            else if (prof.Contains("Knowledge")) prof = "Knowledge";
            else if (prof.Contains("Combat Trickery")) prof = "Combat Trickery";

            temp = read(profsReader);
            while ((temp != prof) && (!profsReader.EndOfStream))
            {
                temp = read(profsReader);
            }
            if (!profsReader.EndOfStream)
            {
                tester = read(profsReader);
            }
            else tester = "ERROR: PROFICIENCY NOT FOUND.";

            //string tester = "The character is trained to jump, tumble, somersault, and free-run around obstacles.  The character gains +2 to saving throws where agility would help avoid the situation, such as tilting floors and pit traps.  In lieu of moving during a round, the character may attempt a proficiency throw of 20+ to tumble behind an opponent in melee.  The profiency throw required for the tumble is reduced by 1 per level of experience the character possesses.  If successful, the character is behind his opponent.  The opponent can now be attacked with a +2 bonus to the attack throw, and gains no benefit from his shield.  Thieves and others eligible to backstab an opponent gain their usual +4 on the attack throw and bonus to damage.  Characters with an encumbrance of 6 stone or more may not tumble.  Note that elven nightblades automatically begin play with this ability as part of their class.";
            while (tester.Contains("."))
            {
                temp = tester.Substring(0, tester.IndexOf(".")+1);
                returner.Add(temp);
                if (tester.IndexOf(".") + 2 > tester.Length)
                {
                    tester = "";
                }
                else tester = tester.Substring(tester.IndexOf(".") + 2);
            }
            profsReader.Close();
            return returner;
        }

        public ArrayList getWeaponDescription(string name)
        {
            return getGenericItemDescription(name, "WeaponDescriptions.txt");
        }

        public ArrayList getItemDescription(string name)
        {
            return getGenericItemDescription(name, "ItemDescriptions.txt");
        }

        public ArrayList getArmorDescription(string name)
        {
            return getGenericItemDescription(name, "ArmorDescriptions.txt");
        }

        private ArrayList getGenericItemDescription(string name, string path)
        {
            ArrayList returner = new ArrayList();
            string temp = "";
            string tester = "";
            System.IO.StreamReader itemReader = new System.IO.StreamReader(Application.StartupPath + "/Resources/Items/" + path);

            temp = read(itemReader);
            while ((temp != name) && (!itemReader.EndOfStream))
            {
                temp = read(itemReader);
            }
            if (!itemReader.EndOfStream)
            {
                tester = read(itemReader);
            }
            else tester = "ERROR: ITEM NOT FOUND.";

            while (tester.Contains("."))
            {
                temp = tester.Substring(0, tester.IndexOf(".") + 1);
                returner.Add(temp);
                if (tester.IndexOf(".") + 2 > tester.Length)
                {
                    tester = "";
                }
                else tester = tester.Substring(tester.IndexOf(".") + 2);
            }
            itemReader.Close();
            return returner;
        }

        public System.Reflection.MethodInfo identifyFunction(string temp)
        {
            Type type = typeof(charClass);
            return type.GetMethod(temp);
        }

        /*public ArrayList getSaves(string type)
        {
            string temp = "get" + saveProgression + type;
            System.Reflection.MethodInfo methodinfo = identifyFunction(temp);
            if (methodinfo != null)
                return (ArrayList)methodinfo.Invoke(this, null);
            //Error handling:  Instead of crashing here, it crashes elsewhere!  ERORR HANDLING!
            return new ArrayList();
        }*/

        //0 = Petrification
        //1 = Poison
        //2 = Blast
        //3 = Staff
        //4 = Spell
        public ArrayList getSaves(int type)
        {
            System.IO.StreamReader saveReader = new System.IO.StreamReader(Application.StartupPath + "/Resources/Classes/Saves.txt");
            line = read(saveReader);
            while ((line != saveProgression) && (!saveReader.EndOfStream))
            {
                line = read(saveReader);
            }
            //I am now where I want to be!  Sort of.
            for (int i = 0; i < type; i++)
            {
                line = read(saveReader);
            }

            line = read(saveReader);

            if (blessed) return assembleThrowsModified(line, 2);
            else if (dwarf)
            {
                //Dwarves get a +3 bonus to blast/breath and a +4 bonus to others
                if (type == 2) return assembleThrowsModified(line, 3);
                else return assembleThrowsModified(line, 4);
            }
            else if (elf)
            {
                //Elves get a +1 bonus to petrification and spells
                if ((type == 0) || (type == 4))
                    return assembleThrowsModified(line, 1);
            }

            return assembleThrows(line);
        }

        public ArrayList assembleThrowsModified(string raw, int modifier)
        {
            string temp = "";
            int tempNumber = -1;
            ArrayList returner = new ArrayList();
            //15+,14+,14+,
            while (raw.Contains(","))
            {
                temp = raw.Substring(0, raw.IndexOf(","));
                tempNumber = System.Convert.ToInt32(temp.Substring(0, temp.IndexOf("+")));
                tempNumber = tempNumber - modifier;
                temp = System.Convert.ToString(tempNumber) + "+";
                returner.Add(temp);
                raw = raw.Substring(raw.IndexOf(",") + 1);
            }
            tempNumber = System.Convert.ToInt32(raw.Substring(0, raw.IndexOf("+")));
            tempNumber = tempNumber - modifier;
            raw = System.Convert.ToString(tempNumber) + "+";
            returner.Add(raw);
            return returner;
        }

        public ArrayList assembleThrows(string raw)
        {
            string temp = "";
            ArrayList returner = new ArrayList();
            //15+,14+,14+,
            while (raw.Contains(","))
            {
                temp = raw.Substring(0, raw.IndexOf(","));
                returner.Add(temp);
                raw = raw.Substring(raw.IndexOf(",") + 1);
            }
            returner.Add(raw);
            return returner;
        }

        public ArrayList getAttackThrowProgression()
        {
            System.IO.StreamReader attackReader = new System.IO.StreamReader(Application.StartupPath + "/Resources/Classes/Attacks.txt");
            line = read(attackReader);
            while (line != System.Convert.ToString(fighting) && (!attackReader.EndOfStream))
            {
                line = read(attackReader);
            }

            line = read(attackReader);
            return assembleThrows(line);
        }



    }
}
