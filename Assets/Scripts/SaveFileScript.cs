using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveFileScript : MonoBehaviour
{
    public static List<ItemScript> items;

    private static string pathToData;

    //method to save the game to a file
    public static void SaveFile(int _floor, int _health, int _charges, int _money, List<ItemScript> _inv)
    {
        //unity file
        pathToData = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));

        //check to make sure there is a folder before making it
        if(!Directory.Exists(pathToData + "/Files/SaveFile/"))
        {
            Directory.CreateDirectory(pathToData + "/Files/SaveFile/");
        }

        pathToData += "/Files/SaveFile/";

        //create the files
        string IntFileName = pathToData + "PlayerData.txt";
        string ItemFileName = pathToData + "PlayerItems.txt";
        string combinedStringInt = _floor.ToString() + "\n" + _health.ToString() + "\n" + _charges.ToString() + "\n" + _money.ToString();
        
        string combinedStringItem = "";
        //create the data for the items
        for(int i = 0; i < _inv.Count; i++)
        {
            if (_inv[i] != null)
            {
                combinedStringItem += _inv[i].item_name + "\n";
            }
        }

        //save the information to the files
        if(File.Exists(IntFileName))
        {
            File.Delete(IntFileName);
        }
        File.WriteAllText(IntFileName, combinedStringInt);

        if (File.Exists(ItemFileName))
        {
            File.Delete(ItemFileName);
        }
        File.WriteAllText(ItemFileName, combinedStringItem);

    }

    //returns an inventory with all the items in the save file
    public static List<ItemScript> LoadItems()
    {
        //get the initial directory path
        pathToData = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));

        //generate an empty list to be filled
        items = new List<ItemScript>();

        //if there is no save file signify that by returning null
        if(!Directory.Exists(pathToData + "/Files/SaveFile/"))
        {
            return null;
        }
        else
        {
            pathToData += "/Files/SaveFile/";
            string file_content = File.ReadAllText(pathToData + "PlayerItems.txt");
            string[] content_lines = file_content.Split("\n");

            //checks each line and adds an item from the database to it
            if(!(content_lines.Length <= 1))
            {
                for (int i = 0; i < content_lines.Length; i++)
                {
                    //get the item name
                    string item_name = content_lines[i].Trim();
                    
                    //check every item to find the one that the name matches up with
                    foreach(GameObject _item in DataManager.items)
                    {
                        if(item_name == _item.GetComponent<ItemScript>().item_name)
                        {
                            items.Add(_item.GetComponent<ItemScript>());
                        }
                    }
                }
            }
        }

        return items;
    }


    public static int LoadFloorNumber()
    {
        //get the initial directory path
        pathToData = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));

        //if there is no save file signify that by returning null
        if (!Directory.Exists(pathToData + "/Files/SaveFile/"))
        {
            return -1;
        }
        else
        {
            pathToData += "/Files/SaveFile/";
            string file_content = File.ReadAllText(pathToData + "PlayerItems.txt");
            string[] content_lines = file_content.Split("\n");

            //try to parse it to make sure its a real number
            if (!int.TryParse(content_lines[0].Trim(), out _))
            {
                return -1;
            }
            else
            {
                //return the floor number
                return int.Parse(content_lines[0].Trim());
            }
        }
    }

    public static int LoadHealth()
    {
        //get the initial directory path
        pathToData = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));

        //if there is no save file signify that by returning null
        if (!Directory.Exists(pathToData + "/Files/SaveFile/"))
        {
            return -1;
        }
        else
        {
            pathToData += "/Files/SaveFile/";
            string file_content = File.ReadAllText(pathToData + "PlayerItems.txt");
            string[] content_lines = file_content.Split("\n");

            //try to parse it to make sure its a real number
            if (!int.TryParse(content_lines[1].Trim(), out _))
            {
                return -1;
            }
            else
            {
                //return the health number
                return int.Parse(content_lines[1].Trim());
            }
        }
    }

    public static int LoadCharges()
    {
        //get the initial directory path
        pathToData = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));

        //if there is no save file signify that by returning null
        if (!Directory.Exists(pathToData + "/Files/SaveFile/"))
        {
            return -1;
        }
        else
        {
            pathToData += "/Files/SaveFile/";
            string file_content = File.ReadAllText(pathToData + "PlayerItems.txt");
            string[] content_lines = file_content.Split("\n");

            //try to parse it to make sure its a real number
            if (!int.TryParse(content_lines[2].Trim(), out _))
            {
                return -1;
            }
            else
            {
                //return the charges number
                return int.Parse(content_lines[2].Trim());
            }
        }
    }

    public static int LoadMoney()
    {
        //get the initial directory path
        pathToData = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));

        //if there is no save file signify that by returning null
        if (!Directory.Exists(pathToData + "/Files/SaveFile/"))
        {
            return -1;
        }
        else
        {
            pathToData += "/Files/SaveFile/";
            string file_content = File.ReadAllText(pathToData + "PlayerItems.txt");
            string[] content_lines = file_content.Split("\n");

            //try to parse it to make sure its a real number
            if (!int.TryParse(content_lines[3].Trim(), out _))
            {
                return -1;
            }
            else
            {
                //return the money number
                return int.Parse(content_lines[3].Trim());
            }
        }
    }

    //saving and loading audio
    public static void LoadAudio()
    {
        //get the initial directory path
        pathToData = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));

        //if there is no save file signify that by returning null
        if (!Directory.Exists(pathToData + "/Files/Settings/"))
        {
            Debug.Log("There is not audio data");
            AudioManager.instance.music_volume = 0.5f;
            AudioManager.instance.sfx_volume = 1.0f;
        }
        else
        {
            pathToData += "/Files/Settings/";
            string file_content = File.ReadAllText(pathToData + "Audio.txt");
            string[] content_lines = file_content.Split("\n");

            //try to parse it to make sure its a real number
            if (!float.TryParse(content_lines[0].Trim(), out _) || !float.TryParse(content_lines[1].Trim(), out _))
            {
                Debug.Log("There is no audio data");
            }
            else
            {
                AudioManager.instance.music_volume = float.Parse(content_lines[0].Trim());
                AudioManager.instance.sfx_volume = float.Parse(content_lines[1].Trim());
            }
        }
    }

    public static void SaveAudio()
    {
        //unity file
        pathToData = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));

        //check to make sure there is a folder before making it
        if (!Directory.Exists(pathToData + "/Files/Settings/"))
        {
            Directory.CreateDirectory(pathToData + "/Files/Settings/");
        }

        pathToData += "/Files/Settings/";

        //create the files
        string AudioFileName = pathToData + "Audio.txt";
        string combinedStringAudio = AudioManager.instance.music_volume + "\n" + AudioManager.instance.sfx_volume;

        //save the information to the files
        if (File.Exists(AudioFileName))
        {
            File.Delete(AudioFileName);
        }

        File.WriteAllText(AudioFileName, combinedStringAudio);
    }
}
