using System;
using System.Collections.Generic;
using System.Linq;

namespace Snrk.Yugioh
{
    public class DatabaseTranslator : IDisposable
    {
        /// <summary>
        /// The format of the description. Available parameters: 
        /// {PendEffectDesc}, {PendEffect}, {CardEffectDesc}, {CardEffect}
        /// </summary>
        public string DescriptionFormat = @"[ {PendEffectDesc} ]\n{PendEffect}\n" +
            "----------------------------------------\n[ {MonsterEffectDesc} ]\n{MonsterEffect}";

        #region private Dictionary<int, Dictionary<string, string>> m_Races
        private Dictionary<int, Dictionary<string, string>> m_Races = new Dictionary<int, Dictionary<string, string>>()
        {
            {
                1,
                new Dictionary<string, string>() {
                    { "ja", "武者" },
                    { "en", "Warrior" },
                    { "de", "Krieger" },
                    { "it", "Guerriero" },
                    { "fr", "Guerrier" },
                    { "es", "Guerrero" },
                    { "tw", "戰士" },
                    { "cn", "战士" }
                }
            },
            {
                2,
                new Dictionary<string, string>() {
                    { "ja", "魔法使い" },
                    { "en", "Spellcaster" },
                    { "de", "Hexer" },
                    { "it", "Incantatore" },
                    { "fr", "Magicien" },
                    { "es", "Lanzador de Conjuros" },
                    { "tw", "施法" },
                    { "cn", "施法" }
                }
            },
            {
                4,
                new Dictionary<string, string>() {
                    { "ja", "妖精" },
                    { "en", "Fairy" },
                    { "de", "Fee" },
                    { "it", "Fata" },
                    { "fr", "Elfe" },
                    { "es", "De hadas" },
                    { "tw", "仙女" },
                    { "cn", "仙女" }
                }
            },
            {
                8,
                new Dictionary<string, string>() {
                    { "ja", "魔" },
                    { "en", "Fiend" },
                    { "de", "Unterweltler" },
                    { "it", "Demone" },
                    { "fr", "Démon" },
                    { "es", "El demonio" },
                    { "tw", "惡魔" },
                    { "cn", "恶魔" }
                }
            },
            {
                16,
                new Dictionary<string, string>() {
                    { "ja", "ゾンビ" },
                    { "en", "Zombie" },
                    { "de", "Zombie" },
                    { "it", "Zombie" },
                    { "fr", "Zombie" },
                    { "es", "Zombi" },
                    { "tw", "殭屍" },
                    { "cn", "僵尸" }
                }
            },
            {
                32,
                new Dictionary<string, string>() {
                    { "ja", "機械" },
                    { "en", "Machine" },
                    { "de", "Maschine" },
                    { "it", "Macchina" },
                    { "fr", "Machine" },
                    { "es", "Máquina" },
                    { "tw", "機" },
                    { "cn", "机" }
                }
            },
            {
                64,
                new Dictionary<string, string>() {
                    { "ja", "アクア" },
                    { "en", "Aqua" },
                    { "de", "Aqua" },
                    { "it", "Acqua" },
                    { "fr", "Aqua" },
                    { "es", "Aqua" },
                    { "tw", "水族" },
                    { "cn", "水族" }
                }
            },
            {
                128,
                new Dictionary<string, string>() {
                    { "ja", "乾式再" },
                    { "en", "Pyro" },
                    { "de", "Pyro" },
                    { "it", "Pyro" },
                    { "fr", "Pyro" },
                    { "es", "Pyro" },
                    { "tw", "火焰兵" },
                    { "cn", "火焰兵" }
                }
            },
            {
                256,
                new Dictionary<string, string>() {
                    { "ja", "岩" },
                    { "en", "Rock" },
                    { "de", "Fels" },
                    { "it", "Roccia" },
                    { "fr", "Rocher" },
                    { "es", "Rock" },
                    { "tw", "岩" },
                    { "cn", "岩" }
                }
            },
            {
                512,
                new Dictionary<string, string>() {
                    { "ja", "翼の獣" },
                    { "en", "Winged Beast" },
                    { "de", "Geflügeltes Ungeheuer" },
                    { "it", "Bestia Alata" },
                    { "fr", "Bête-Ailée" },
                    { "es", "Bestia Alada" },
                    { "tw", "翼獸" },
                    { "cn", "翼兽" }
                }
            },
            {
                1024,
                new Dictionary<string, string>() {
                    { "ja", "植物" },
                    { "en", "Plant" },
                    { "de", "Pflanze" },
                    { "it", "Pianta" },
                    { "fr", "Plante" },
                    { "es", "Planta" },
                    { "tw", "廠" },
                    { "cn", "厂" }
                }
            },
            {
                2048,
                new Dictionary<string, string>() {
                    { "ja", "昆虫" },
                    { "en", "Insect" },
                    { "de", "Insekt" },
                    { "it", "Insetto" },
                    { "fr", "Insecte" },
                    { "es", "Insecto" },
                    { "tw", "昆蟲" },
                    { "cn", "" }
                }
            },
            {
                4096,
                new Dictionary<string, string>() {
                    { "ja", "雷" },
                    { "en", "Thunder" },
                    { "de", "Donner" },
                    { "it", "Tuono" },
                    { "fr", "Tonnerre" },
                    { "es", "Thunder" },
                    { "tw", "雷" },
                    { "cn", "昆虫" }
                }
            },
            {
                8192,
                new Dictionary<string, string>() {
                    { "ja", "竜" },
                    { "en", "Dragon" },
                    { "de", "Drache" },
                    { "it", "Drago" },
                    { "fr", "Dragon" },
                    { "es", "Dragón" },
                    { "tw", "龍" },
                    { "cn", "龙" }
                }
            },
            {
                16384,
                new Dictionary<string, string>() {
                    { "ja", "獣" },
                    { "en", "Beast" },
                    { "de", "Ungeheuer" },
                    { "it", "Bestia" },
                    { "fr", "Bête" },
                    { "es", "Bestia" },
                    { "tw", "獸" },
                    { "cn", "兽" }
                }
            },
            {
                32768,
                new Dictionary<string, string>() {
                    { "ja", "獣戦士" },
                    { "en", "Beast-Warrior" },
                    { "de", "Ungeheuer-Krieger" },
                    { "it", "Guerriero-Bestia" },
                    { "fr", "Bête-Guerrier" },
                    { "es", "Bestia-Guerrero" },
                    { "tw", "獸戰士" },
                    { "cn", "兽战士" }
                }
            },
            {
                65536,
                new Dictionary<string, string>() {
                    { "ja", "恐竜" },
                    { "en", "Dinosaur" },
                    { "de", "Dinosaurier" },
                    { "it", "Dinosauro" },
                    { "fr", "Dinosaure" },
                    { "es", "Dinosaurio" },
                    { "tw", "恐龍" },
                    { "cn", "恐龙" }
                }
            },
            {
                131072,
                new Dictionary<string, string>() {
                    { "ja", "魚" },
                    { "en", "Fish" },
                    { "de", "Fisch" },
                    { "it", "Pesce" },
                    { "fr", "Poisson" },
                    { "es", "Pescado" },
                    { "tw", "魚" },
                    { "cn", "鱼" }
                }
            },
            {
                262144,
                new Dictionary<string, string>() {
                    { "ja", "海-大蛇" },
                    { "en", "Sea Serpent" },
                    { "de", "Seeschlange" },
                    { "it", "Serpente Marino" },
                    { "fr", "Serpent de Mer" },
                    { "es", "Serpiente Marina" },
                    { "tw", "蜃" },
                    { "cn", "蜃" }
                }
            },
            {
                524288,
                new Dictionary<string, string>() {
                    { "ja", "爬虫類" },
                    { "en", "Reptile" },
                    { "de", "Reptil" },
                    { "it", "Rettile" },
                    { "fr", "Reptile" },
                    { "es", "Reptiles" },
                    { "tw", "爬蟲" },
                    { "cn", "爬虫" }
                }
            },
            {
                1048576,
                new Dictionary<string, string>() {
                    { "ja", "サイキックアタック" },
                    { "en", "Psychic" },
                    { "de", "Psi" },
                    { "it", "Psichico" },
                    { "fr", "Psychique" },
                    { "es", "Psíquico" },
                    { "tw", "精神" },
                    { "cn", "精神" }
                }
            },
            {
                2097152,
                new Dictionary<string, string>() {
                    { "ja", "神-獣" },
                    { "en", "Divine-Beast" },
                    { "de", "Göttliches Ungeheuer" },
                    { "it", "Divinità-Bestia" },
                    { "fr", "Bête Divine" },
                    { "es", "Divino-Bestia" },
                    { "tw", "神州野獸" },
                    { "cn", "神州野兽" }
                }
            },
            {
                4194304,
                new Dictionary<string, string>() {
                    { "ja", "創造主の神" },
                    { "en", "Creator God" },
                    { "de", "Schöpfer-Gott" },
                    { "it", "Dio Creatore" },
                    { "fr", "Dieu Créateur" },
                    { "es", "Dios Creador" },
                    { "tw", "造物主" },
                    { "cn", "造物主" }
                }
            },
            {
                8388608,
                new Dictionary<string, string>() {
                    { "ja", "飛竜" },
                    { "en", "Wyrm" },
                    { "de", "Wyrm" },
                    { "it", "Wyrm" },
                    { "fr", "Wyrm" },
                    { "es", "Wyrm" },
                    { "tw", "巨龍" },
                    { "cn", "巨龙" }
                }
            }
        };
        #endregion

        #region private Dictionary<int, Dictionary<string, string>> m_Types;
        private Dictionary<int, Dictionary<string, string>> m_Types = new Dictionary<int, Dictionary<string, string>>()
        {
            {
                1,
                new Dictionary<string, string>() {
                    { "ja", "モンスタ" },
                    { "en", "Monster" },
                    { "de", "Monster" },
                    { "it", "Mostro" },
                    { "fr", "Monstre" },
                    { "es", "Monstruo" },
                    { "tw", "怪物" },
                    { "cn", "怪物" }
                }
            },
            {
                2,
                new Dictionary<string, string>() {
                    { "ja", "スペル" },
                    { "en", "Spell" },
                    { "de", "Zauber" },
                    { "it", "Magia" },
                    { "fr", "Sort" },
                    { "es", "Hechizo" },
                    { "tw", "拼寫" },
                    { "cn", "拼写" }
                }
            },
            {
                4,
                new Dictionary<string, string>() {
                    { "ja", "トラップ" },
                    { "en", "Trap" },
                    { "de", "Falle" },
                    { "it", "Trappola" },
                    { "fr", "Piège" },
                    { "es", "Trampa" },
                    { "tw", "陷阱" },
                    { "cn", "陷阱" }
                }
            },
            {
                16,
                new Dictionary<string, string>() {
                    { "ja", "通常の" },
                    { "en", "Normal" },
                    { "de", "Normal" },
                    { "it", "Normale" },
                    { "fr", "Normal" },
                    { "es", "Normal" },
                    { "tw", "正常" },
                    { "cn", "正常" }
                }
            },
            {
                32,
                new Dictionary<string, string>() {
                    { "ja", "効果" },
                    { "en", "Effect" },
                    { "de", "Effekt" },
                    { "it", "Effetto" },
                    { "fr", "Effet" },
                    { "es", "Efecto" },
                    { "tw", "影響" },
                    { "cn", "影响" }
                }
            },
            {
                64,
                new Dictionary<string, string>() {
                    { "ja", "核融合" },
                    { "en", "Fusion" },
                    { "de", "Fusion" },
                    { "it", "Fusione" },
                    { "fr", "Fusion" },
                    { "es", "Fusión" },
                    { "tw", "聚變" },
                    { "cn", "聚变" }
                }
            },
            {
                128,
                new Dictionary<string, string>() {
                    { "ja", "儀式" },
                    { "en", "Ritual" },
                    { "de", "Ritual" },
                    { "it", "Rituale" },
                    { "fr", "Rituel" },
                    { "es", "Ritual" },
                    { "tw", "儀式" },
                    { "cn", "仪式" }
                }
            },
            {
                512,
                new Dictionary<string, string>() {
                    { "ja", "精神" },
                    { "en", "Spirit" },
                    { "de", "Spirit" },
                    { "it", "Spirit" },
                    { "fr", "Spirit" },
                    { "es", "Espíritu" },
                    { "tw", "精神" },
                    { "cn", "精神" }
                }
            },
            {
                1024,
                new Dictionary<string, string>() {
                    { "ja", "組合" },
                    { "en", "Union" },
                    { "de", "Union" },
                    { "it", "Unione" },
                    { "fr", "Union" },
                    { "es", "Unión" },
                    { "tw", "聯盟" },
                    { "cn", "联盟" }
                }
            },
            {
                2048,
                new Dictionary<string, string>() {
                    { "ja", "ジェミニ" },
                    { "en", "Gemini" },
                    { "de", "Zwilling" },
                    { "it", "Gemello" },
                    { "fr", "Gémeau" },
                    { "es", "Géminis" },
                    { "tw", "雙子座" },
                    { "cn", "双子座" }
                }
            },
            {
                4096,
                new Dictionary<string, string>() {
                    { "ja", "チューナー" },
                    { "en", "Tuner" },
                    { "de", "Empfänger" },
                    { "it", "Sintonizzatore" },
                    { "fr", "Syntoniseur" },
                    { "es", "Sintonizador" },
                    { "tw", "調諧器" },
                    { "cn", "调谐器" }
                }
            },
            {
                8192,
                new Dictionary<string, string>() {
                    { "ja", "シンクロ" },
                    { "en", "Synchro" },
                    { "de", "Synchro" },
                    { "it", "Synchro" },
                    { "fr", "Synchro" },
                    { "es", "Synchro" },
                    { "tw", "同步" },
                    { "cn", "同步" }
                }
            },
            {
                16384,
                new Dictionary<string, string>() {
                    { "ja", "トークン" },
                    { "en", "Token" },
                    { "de", "Spielmarke" },
                    { "it", "Segna-Mostro" },
                    { "fr", "Jeton" },
                    { "es", "Token" },
                    { "tw", "象徵" },
                    { "cn", "象征" }
                }
            },
            {
                65536,
                new Dictionary<string, string>() {
                    { "ja", "クイック再生" },
                    { "en", "Quick-Play" },
                    { "de", "Schnell" },
                    { "it", "Rapida" },
                    { "fr", "Jeu-Rapide" },
                    { "es", "Juego Rápido" },
                    { "tw", "快玩" },
                    { "cn", "快玩" }
                }
            },
            {
                131072,
                new Dictionary<string, string>() {
                    { "ja", "継続的な" },
                    { "en", "Continuous" },
                    { "de", "Permanent" },
                    { "it", "Continua" },
                    { "fr", "Continue" },
                    { "es", "Continua" },
                    { "tw", "連續" },
                    { "cn", "连续" }
                }
            },
            {
                262144,
                new Dictionary<string, string>() {
                    { "ja", "装備する" },
                    { "en", "Equip" },
                    { "de", "Ausrüstung" },
                    { "it", "Equipaggiamento" },
                    { "fr", "Équiper" },
                    { "es", "Equipar" },
                    { "tw", "裝備" },
                    { "cn", "装备" }
                }
            },
            {
                524288,
                new Dictionary<string, string>() {
                    { "ja", "分野" },
                    { "en", "Field" },
                    { "de", "Feld" },
                    { "it", "Terreno" },
                    { "fr", "Champ" },
                    { "es", "Campo" },
                    { "tw", "領域" },
                    { "cn", "领域" }
                }
            },
            {
                1048576,
                new Dictionary<string, string>() {
                    { "ja", "カウンター" },
                    { "en", "Counter" },
                    { "de", "Konter" },
                    { "it", "Contro" },
                    { "fr", "Contre" },
                    { "es", "Contador" },
                    { "tw", "計數器" },
                    { "cn", "计数器" }
                }
            },
            {
                2097152,
                new Dictionary<string, string>() {
                    { "ja", "フリップ" },
                    { "en", "Flip" },
                    { "de", "Flip" },
                    { "it", "Scoperta" },
                    { "fr", "Flip" },
                    { "es", "Flip" },
                    { "tw", "翻動" },
                    { "cn", "翻动" }
                }
            },
            {
                4194304,
                new Dictionary<string, string>() {
                    { "ja", "東温" },
                    { "en", "Toon" },
                    { "de", "Toon" },
                    { "it", "Toon" },
                    { "fr", "Toon" },
                    { "es", "Toon" },
                    { "tw", "卡通" },
                    { "cn", "卡通" }
                }
            },
            {
                8388608,
                new Dictionary<string, string>() {
                    { "ja", "Ｘモンスター" },
                    { "en", "Xyz" },
                    { "de", "Xyz" },
                    { "it", "Xyz" },
                    { "fr", "Xyz" },
                    { "es", "Xyz" },
                    { "tw", "超量" },
                    { "cn", "超量" }
                }
            },
            {
                16777216,
                new Dictionary<string, string>() {
                    { "ja", "振り子" },
                    { "en", "Pendulum" },
                    { "de", "Pendel" },
                    { "it", "Pendulum" },
                    { "fr", "Pendule" },
                    { "es", "Péndulo" },
                    { "tw", "擺" },
                    { "cn", "摆" }
                }
            },
            {
                268435456,
                new Dictionary<string, string>() {
                    { "ja", "鎧" },
                    { "en", "Armor" },
                    { "de", "Rüstung" },
                    { "it", "Armatura" },
                    { "fr", "Armure" },
                    { "es", "Armadura" },
                    { "tw", "盔甲" },
                    { "cn", "盔甲" }
                }
            },
            {
                536870912,
                new Dictionary<string, string>() {
                    { "ja", "プラス" },
                    { "en", "Plus" },
                    { "de", "Plus" },
                    { "it", "Più" },
                    { "fr", "Plus" },
                    { "es", "Más" },
                    { "tw", "加" },
                    { "cn", "加" }
                }
            },
            {
                1073741824,
                new Dictionary<string, string>() {
                    { "ja", "マイナス" },
                    { "en", "Minus" },
                    { "de", "Minus" },
                    { "it", "Meno" },
                    { "fr", "Moins" },
                    { "es", "Menos" },
                    { "tw", "減去" },
                    { "cn", "减去" }
                }
            }
        };
        #endregion

        #region private Dictionary<int, Dictionary<string, string>> m_Attributes
        private Dictionary<int, Dictionary<string, string>> m_Attributes = new Dictionary<int, Dictionary<string, string>>()
        {
            {
                1, new Dictionary<string, string>() {
                    { "ja", "地" },
                    { "en", "Earth" },
                    { "de", "Erde" },
                    { "it", "Terra" },
                    { "fr", "Terre" },
                    { "es", "Tierra" },
                    { "tw", "地球" },
                    { "cn", "地球" }
                }
            },
            {
                2, new Dictionary<string, string>() {
                    { "ja", "水" },
                    { "en", "Water" },
                    { "de", "Wasser" },
                    { "it", "Acqua" },
                    { "fr", "Eau" },
                    { "es", "Agua" },
                    { "tw", "水" },
                    { "cn", "水" }
                }
            },
            {
                4, new Dictionary<string, string>() {
                    { "ja", "火" },
                    { "en", "Fire" },
                    { "de", "Feuer" },
                    { "it", "Fuoco" },
                    { "fr", "Feu" },
                    { "es", "Fuego" },
                    { "tw", "火" },
                    { "cn", "火" }
                }
            },
            {
                8, new Dictionary<string, string>() {
                    { "ja", "風" },
                    { "en", "Wind" },
                    { "de", "Wind" },
                    { "it", "Vento" },
                    { "fr", "Vent" },
                    { "es", "Viento" },
                    { "tw", "風" },
                    { "cn", "风" }
                }
            },
            {
                16, new Dictionary<string, string>() {
                    { "ja", "光" },
                    { "en", "Light" },
                    { "de", "Licht" },
                    { "it", "Luce" },
                    { "fr", "Lumière" },
                    { "es", "Luz" },
                    { "tw", "光" },
                    { "cn", "光" }
                }
            },
            {
                32, new Dictionary<string, string>() {
                    { "ja", "暗" },
                    { "en", "Dark" },
                    { "de", "Finsternis" },
                    { "it", "Oscurità" },
                    { "fr", "Ténèbres" },
                    { "es", "Oscuridad" },
                    { "tw", "黑暗" },
                    { "cn", "黑暗" }
                }
            },
            {
                64, new Dictionary<string, string>() {
                    { "ja", "神" },
                    { "en", "Divine" },
                    { "de", "Göttlich" },
                    { "it", "Divino" },
                    { "fr", "Divin" },
                    { "es", "Divinidad" },
                    { "tw", "神聖" },
                    { "cn", "神圣" }
                }
            }
        };
        #endregion

        #region private Dictionary<string, Dictionary<string, string>> m_Descriptions
        private Dictionary<string, Dictionary<string, string>> m_Descriptions = new Dictionary<string, Dictionary<string, string>>()
        {
            {
                "Illegal",
                new Dictionary<string, string>() {
                    { "ja", "違法" },
                    { "en", "Illegal" },
                    { "de", "Illegal" },
                    { "fr", "Illégal" },
                    { "it", "Illegale" },
                    { "es", "Ilegal" },
                    { "tw", "非法" },
                    { "cn", "非法" }
                }
            },
            {
                "Custom",
                new Dictionary<string, string>() {
                    { "ja", "Custom" },
                    { "en", "Custom" },
                    { "de", "Custom" },
                    { "fr", "Custom" },
                    { "it", "Custom" },
                    { "es", "Custom" },
                    { "tw", "Custom" },
                    { "cn", "Custom" }
                }
            },
            {
                "Price",
                new Dictionary<string, string>() {
                    { "ja", "価格" },
                    { "en", "Price" },
                    { "de", "Preis" },
                    { "fr", "Prix" },
                    { "it", "Prezzo" },
                    { "es", "Precio" },
                    { "tw", "價錢" },
                    { "cn", "价钱" }
                }
            },
            {
                "FlavorText",
                new Dictionary<string, string>() {
                    { "ja", "説明" },
                    { "en", "Flavor Text" },
                    { "de", "Beschreibung" },
                    { "fr", "Description" },
                    { "it", "Descrizione" },
                    { "es", "Descripción" },
                    { "tw", "描述" },
                    { "cn", "描述" }
                }
            },
            {
                "CardText",
                new Dictionary<string, string>() {
                    { "ja", "カードテキスト" },
                    { "en", "Card Text" },
                    { "de", "Kartentext" },
                    { "fr", "Texte de Carte" },
                    { "it", "Testo Carta" },
                    { "es", "Texto de Carta" },
                    { "tw", "卡文" },
                    { "cn", "卡文" }
                }
            },
            {
                "Effect",
                new Dictionary<string, string>() {
                    { "ja", "効果" },
                    { "en", "Effect" },
                    { "de", "Effekt" },
                    { "fr", "Effet" },
                    { "it", "Effetto" },
                    { "es", "Efecto" },
                    { "tw", "影響" },
                    { "cn", "影响" }
                }
            },
            {
                "MonsterEffect",
                new Dictionary<string, string>() {
                    { "ja", "モンスター効果" },
                    { "en", "Monster Effect" },
                    { "de", "Monstereffekt" },
                    { "fr", "Effet de Monstre" },
                    { "it", "Effetto Mostro" },
                    { "es", "Efecto de Monstruo" },
                    { "tw", "怪兽效果" },
                    { "cn", "怪兽效果" }
                }
            },
            {
                "PendulumEffect",
                new Dictionary<string, string>() {
                    { "ja", "ペンデュラム効果" },
                    { "en", "Pendulum Effect" },
                    { "de", "Pendeleffekt" },
                    { "fr", "Effet Pendule" },
                    { "it", "Effetto Pendulum" },
                    { "es", "Efecto de Péndulo" },
                    { "tw", "鐘擺效應" },
                    { "cn", "钟摆效应" }
                }
            },
            {
                "PendulumScale",
                new Dictionary<string, string>() {
                    { "ja", "ペンデュラムスケール" },
                    { "en", "Pendulum Scale" },
                    { "de", "Pendelbereich" },
                    { "fr", "Échelle Pendule" },
                    { "it", "Valore Pendulum" },
                    { "es", "Escala de Péndulo" },
                    { "tw", "擺秤" },
                    { "cn", "摆秤" }
                }
            },
            {
                "Attribute",
                new Dictionary<string, string>() {
                    { "ja", "属性" },
                    { "en", "Attribute" },
                    { "de", "Attribut" },
                    { "fr", "Attribut" },
                    { "it", "Attributo" },
                    { "es", "Atributo" },
                    { "tw", "屬性" },
                    { "cn", "属性" }
                }
            },
            {
                "TypeSingular",
                new Dictionary<string, string>() {
                    { "ja", "タイプ" },
                    { "en", "Type" },
                    { "de", "Typ" },
                    { "fr", "Type" },
                    { "it", "Tipo" },
                    { "es", "Tipo" },
                    { "tw", "類型" },
                    { "cn", "类型" }
                }
            },
            {
                "TypePlural",
                new Dictionary<string, string>() {
                    { "ja", "タイプ" },
                    { "en", "Types" },
                    { "de", "Typen" },
                    { "fr", "Types" },
                    { "it", "Tipi" },
                    { "es", "Tipos" },
                    { "tw", "類型" },
                    { "cn", "类型" }
                }
            },
            {
                "Level",
                new Dictionary<string, string>() {
                    { "ja", "レベル" },
                    { "en", "Level" },
                    { "de", "Level" },
                    { "fr", "Niveau" },
                    { "it", "Livello" },
                    { "es", "Nivel" },
                    { "tw", "水平" },
                    { "cn", "水平" }
                }
            },
            {
                "Rank",
                new Dictionary<string, string>() {
                    { "ja", "ランク" },
                    { "en", "Rank" },
                    { "de", "Rang" },
                    { "fr", "Rang" },
                    { "it", "Rango" },
                    { "es", "Rango" },
                    { "tw", "等級" },
                    { "cn", "等级" }
                }
            },
            {
                "LimitationSingular",
                new Dictionary<string, string>() {
                    { "ja", "制限" },
                    { "en", "Limitation" },
                    { "de", "Limitierung" },
                    { "fr", "Limitation" },
                    { "it", "Limitazione" },
                    { "es", "Limitación" },
                    { "tw", "局限性" },
                    { "cn", "局限性" }
                }
            },
            {
                "LimitationPlural",
                new Dictionary<string, string>() {
                    { "ja", "制限事項" },
                    { "en", "Limitations" },
                    { "de", "Limitierungen" },
                    { "fr", "Limites" },
                    { "it", "Limitazioni" },
                    { "es", "Limitaciones" },
                    { "tw", "限制" },
                    { "cn", "限制" }
                }
            }
        };
        #endregion
        
        ~DatabaseTranslator() { Dispose(); }
        public void Dispose()
        {
            if (m_Attributes != null)
            {
                var attributeKeys = m_Attributes.Keys.ToArray();
                for (int i = 0; i < attributeKeys.Length; ++i)
                {
                    m_Attributes[attributeKeys[i]]?.Clear();
                    m_Attributes[attributeKeys[i]] = null;
                }
                m_Attributes = null;
            }
            if (m_Descriptions != null)
            {
                var descriptionKeys = m_Descriptions.Keys.ToArray();
                for (int i = 0; i < descriptionKeys.Length; ++i)
                {
                    m_Descriptions[descriptionKeys[i]]?.Clear();
                    m_Descriptions[descriptionKeys[i]] = null;
                }
                m_Descriptions = null;
            }
            if (m_Races != null)
            {
                var raceKeys = m_Races.Keys.ToArray();
                for (int i = 0; i < raceKeys.Length; ++i)
                {
                    m_Races[raceKeys[i]]?.Clear();
                    m_Races[raceKeys[i]] = null;
                }
                m_Races = null;
            }
            if (m_Types != null)
            {
                var typeKeys = m_Types.Keys.ToArray();
                for (int i = 0; i < typeKeys.Length; ++i)
                {
                    m_Types[typeKeys[i]]?.Clear();
                    m_Types[typeKeys[i]] = null;
                }
                m_Types = null;
            }
        }

        public DatabaseCard.Effect ParseEffects(string description, string language)
        {
            DatabaseCard.Effect effect = new DatabaseCard.Effect();

            if (String.IsNullOrEmpty(description))
            {
                return effect;
            }

            int indexEffect = description.IndexOf(string.Format("[ {0} ]", m_Descriptions["MonsterEffect"][language]));
            int indexFlavor = description.IndexOf(string.Format("[ {0} ]", m_Descriptions["FlavorText"][language]));
            int indexPendEffect = description.IndexOf(string.Format("[ {0} ]", m_Descriptions["PendulumEffect"][language]));
            int indexDeckMasterEffect = description.IndexOf("[ Deck Master Effect ]");
            int indexBarrier = description.IndexOf("---");

            effect.CardEffect = description.Substring(indexEffect > -1 ? indexEffect + m_Descriptions["MonsterEffect"][language].Length + 4 :
                (indexFlavor > -1 ? indexFlavor + m_Descriptions["FlavorText"][language].Length + 4 : 0)).Trim();
            if (indexPendEffect > -1 && indexBarrier > -1)
                effect.PendulumEffect = description.Substring(indexPendEffect + m_Descriptions["PendulumEffect"][language].Length + 4,
                    indexBarrier - indexPendEffect - m_Descriptions["PendulumEffect"][language].Length - 4).Trim();
            else if (indexDeckMasterEffect > -1 && indexBarrier > -1)
            {
                effect.DeckMasterEffect = description.Substring(indexDeckMasterEffect + "Deck Master Effect".Length + 4,
                    indexBarrier - indexDeckMasterEffect - "Deck Master Effect".Length - 4).Trim();
            }

            return effect;
        }
        
        public string ToDescriptionString(DatabaseCard.Effect effect, string language/*, string pendulumScale = ""*/)
        {
            if (string.IsNullOrEmpty(effect.DeckMasterEffect) && string.IsNullOrEmpty(effect.PendulumEffect))
            {
                return effect.CardEffect;
            }
            return this.DescriptionFormat
                //.Replace("{PendScaleDesc}", m_Descriptions["PendulumScale"][language])
                .Replace("{PendEffectDesc}", m_Descriptions["PendulumEffect"][language])
                .Replace("{MonsterEffectDesc}", m_Descriptions["MonsterEffect"][language])
                //.Replace("{PendScale}", pendulumScale == null ? "" : pendulumScale)
                .Replace("{PendEffect}", !string.IsNullOrEmpty(effect.DeckMasterEffect) ? effect.DeckMasterEffect : effect.PendulumEffect)
                .Replace("{MonsterEffect}", effect.CardEffect);
        }

        public string ToAppearanceString(int ot)
        {
            return ToAppearanceString(ot.ToString());
        }

        public string ToAppearanceString(string ot)
        {
            switch (ot)
            {
                case "1": return "OCG";
                case "2": return "TCG";
                case "3": return "OCG/TCG";
                case "4": return "Anime";
                default: return null;
            }
        }
    }
}
