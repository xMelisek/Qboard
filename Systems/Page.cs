namespace Qboard.Systems;

public class Page
{
    public string name;
    public string[] sounds;

    public Page(string name, string[] sounds)
    {
        //Load the config file
        //Get the page with that name
        //Load respectively the sound paths
        this.name = name;
        this.sounds = sounds;
    }

    public void SetSound(int index, string sound)
    {
        sounds[index] = sound;
    }
}