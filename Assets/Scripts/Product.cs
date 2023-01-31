using UnityEngine;

public struct Plan
{
    public (int current, int max) plan;
    public (int current, int max) develop;
    public (int current, int max) art;

    public (float current, float max) originality;
    public (float current, float max) graphic;
    public (float current, float max) completeness;

    public float originalityResult;
    public float graphicResult;
    public float completenessResult;

    public void Init(int _plan, int _dev, int _art)
    {
        plan = (0, _plan);
        develop = (0, _dev);
        art = (0, _art);

        originality = (0f, 0f);
        graphic = (0f, 0f);
        completeness = (0f, 0f);
    }

    public float CheckProgress()
    {
        float p = Utils.GetTupleRatio(plan);
        float d = Utils.GetTupleRatio(develop);
        float a = Utils.GetTupleRatio(art);
        return (p + d + a) / 3;
    }

    public float Evaluate()
    {
        originalityResult = Utils.GetTupleRatio(originality);
        graphicResult = Utils.GetTupleRatio(graphic);
        completenessResult = Utils.GetTupleRatio(completeness);
        return (originalityResult + graphicResult + completenessResult) / 3;
    }

    public void PrintPlan()
    {
        Debug.Log($"��ȹ: {plan} ����: {develop} ��Ʈ: {art}");
    }
}

public class Product
{
    public Plan prodPlan;

    public void SetPlan(int plan, int dev, int art)
    {
        prodPlan.Init(plan, dev, art);
    }
}