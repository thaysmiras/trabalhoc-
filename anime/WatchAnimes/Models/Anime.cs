namespace Videos.Models
{
    public Anime(string name, string gender, bool premium, int classification, string ultimoEp)
    {
        id = Guid.NewGuid();
        name = name;
        gender = gender;
        premium = premium;
        classification = classification;
        ultimoEp = ultimoEp;
    }}