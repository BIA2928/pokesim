using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeImageDB : MonoBehaviour
{
    [SerializeField] Sprite fairySprite;
    [SerializeField] Sprite bugSprite;
    [SerializeField] Sprite electricSprite;
    [SerializeField] Sprite fireSprite;
    [SerializeField] Sprite grassSprite;
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite rockSprite;
    [SerializeField] Sprite darkSprite;
    [SerializeField] Sprite flyingSprite;
    [SerializeField] Sprite groundSprite;
    [SerializeField] Sprite poisonSprite;
    [SerializeField] Sprite steelSprite;
    [SerializeField] Sprite dragonSprite;
    [SerializeField] Sprite fightingSprite;
    [SerializeField] Sprite ghostSprite;
    [SerializeField] Sprite iceSprite;
    [SerializeField] Sprite psychicSprite;
    [SerializeField] Sprite waterSprite;
    [SerializeField] Sprite noneSprite;
    public static TypeImageDB i { get; private set; }
    Dictionary<PokeType, Sprite> spriteDict;
    // Start is called before the first frame update

    private void Awake()
    {
        i = this;
    }

    public void Init()
    {
        i = this;
        spriteDict = new Dictionary<PokeType, Sprite>();
        spriteDict[PokeType.Bug] = bugSprite;
        spriteDict[PokeType.Fairy] = fairySprite;
        spriteDict[PokeType.Electric] = electricSprite;
        spriteDict[PokeType.None] = noneSprite;
        spriteDict[PokeType.Fire] = fireSprite;
        spriteDict[PokeType.Fighting] = fightingSprite;
        spriteDict[PokeType.Grass] = grassSprite;
        spriteDict[PokeType.Normal] = normalSprite;
        spriteDict[PokeType.Rock] = rockSprite;
        spriteDict[PokeType.Dark] = darkSprite;
        spriteDict[PokeType.Flying] = flyingSprite;
        spriteDict[PokeType.Ground] = groundSprite;
        spriteDict[PokeType.Poison] = poisonSprite;
        spriteDict[PokeType.Steel] = steelSprite;
        spriteDict[PokeType.Dragon] = dragonSprite;
        spriteDict[PokeType.Ghost] = ghostSprite;
        spriteDict[PokeType.Ice] = iceSprite;
        spriteDict[PokeType.Psychic] = psychicSprite;
        spriteDict[PokeType.Water] = waterSprite;
        spriteDict[PokeType.None] = noneSprite;
    }

    public Sprite Lookup(PokeType type)
    {
        if (!spriteDict.ContainsKey(type))
        {
            Debug.LogError("type image lookup error!");
            return null;
        }
        return spriteDict[type];
    }
}
