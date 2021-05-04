namespace UniRxSampleGame.Damages
{
    // ダメージを与えることができることを示すインタフェース
    public interface IDamageApplicable
    {
        void ApplyDamage(in Damage damage);
    }
}