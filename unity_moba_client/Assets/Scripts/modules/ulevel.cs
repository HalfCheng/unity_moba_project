using UnityEngine;

public class ulevel : Singleton<ulevel>
{
    private int[] level_exp;
    public void Init()
    {
        this.level_exp = new int[]
        {
            0,
            1000,
            2000,
            3000,
            4000,
            5000,
            6000,
            7000,
            8000,
            9000,
            10000,
            20000,
        };
    }

    public int get_level_info(int uexp, out int now_exp, out int next_level_exp)
    {
        now_exp = 0;
        next_level_exp = 0;

        int level = 0;
        int last_exp = uexp;

        while (level + 1 <= this.level_exp.Length - 1 && last_exp>= this.level_exp[level + 1])
        {
            last_exp -= this.level_exp[level + 1];
            level++;
        }
        
        now_exp = last_exp;
        if (level == this.level_exp.Length - 1)
        {
            next_level_exp = now_exp;
        }
        else
        {
            next_level_exp = this.level_exp[level + 1];
        }
        
        return level;
    }
}