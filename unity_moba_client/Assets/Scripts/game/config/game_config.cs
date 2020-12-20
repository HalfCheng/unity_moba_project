using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tower_config
{
    public int hp; // 生命力
    public int defense; // 塔的防御
    public int attack_R; // 攻击范围
    public int attack; // 攻击力
    public int speed; // 子弹移动的速度;
    public int max_distance; // 最大有效范围是多少;
    public int shoot_logic_fps; // 子弹频率
}

// 用多种英雄，每种英雄15个等级;
public class hero_level_config
{
    public int defense; // 英雄的防御;

    // public kill_wound attack; // 普通攻击
    // public kill_wound skill; // 技能攻击;
    public int max_blood; // 当前等级英雄最大血量;
    public int add_blood; // 升到这级的时候要给英雄加多少血量;
    public int exp;
    public int relive_fps; // 英雄在这个登记的复合时间;
}


public class game_config
{
    public static tower_config main_tower_config = new tower_config()
    {
        hp = 200,
        defense = 10,
        attack_R = 10,
        attack = 10,
        speed = 20,
        max_distance = 20,
        shoot_logic_fps = 3, // 1 秒5个子弹
    };

    public static tower_config normal_tower_config = new tower_config()
    {
        hp = 100,
        defense = 10,
        attack_R = 10,
        attack = 10,
        speed = 20,
        max_distance = 20,
        shoot_logic_fps = 5, // 1 秒5个子弹
    };

    public static hero_level_config[] normal_hero_level_config = new hero_level_config[]
    {
        new hero_level_config()
        {
            defense = 1,
            // attack = new kill_wound()
            // {
            //     value = 20,
            //     valid_R = 2.5f,
            // },
            //
            // skill = new kill_wound()
            // {
            //     value = 30,
            //     valid_R = 2.5f
            // },

            max_blood = 100,
            add_blood = 0,
            exp = 0, // 增加的经验
            relive_fps = 7 * 15, // 5秒
        },

        new hero_level_config()
        {
            defense = 2,
            // attack = new kill_wound()
            // {
            //     value = 20,
            //     valid_R = 2.5f,
            // },
            //
            // skill = new kill_wound()
            // {
            //     value = 30,
            //     valid_R = 2.5f
            // },

            max_blood = 200,
            add_blood = 50,
            exp = 100, // 增加的经验
            relive_fps = 7 * 15, // 5秒复活
        },

        new hero_level_config()
        {
            defense = 3,
            // attack = new kill_wound()
            // {
            //     value = 20,
            //     valid_R = 2.5f,
            // },
            //
            // skill = new kill_wound()
            // {
            //     value = 30,
            //     valid_R = 2.5f
            // },
            max_blood = 300,
            add_blood = 100,
            exp = 200, // 增加的经验
            relive_fps = 7 * 15, // 5秒
        },
    };

    public static int add_exp_per_logic = 1; // 每一个逻辑帧成长1点;
    public static int gen_monster_frames = 15 * 33; // 15 * 33

    public static int exp2level(hero_level_config[] configs, int exp)
    {
        // 所有的exp
        int level = 0; // 从第0级开始
        while (level + 1 < configs.Length && exp >= configs[level + 1].exp)
        {
            exp -= (configs[level + 1].exp);
            level++;
        }

        return level;
    }

    public static void exp_upgrade_level_info(hero_level_config[] configs, int exp, ref int now, ref int total)
    {
        int level = 0; // 从第0级开始
        while (level + 1 < configs.Length && exp >= configs[level + 1].exp)
        {
            exp -= (configs[level + 1].exp);
            level++;
        }

        if (level + 1 >= configs.Length)
        {
            now = total = configs[level].exp;
        }
        else
        {
            now = exp;
            total = configs[level + 1].exp;
        }
    }
}