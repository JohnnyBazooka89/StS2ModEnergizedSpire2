using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;

namespace EnergizedSpire2.EnergizedSpire2Code.Powers;

public abstract class EnergizedSpire2TemporaryPower<TModel, TPower> : CustomTemporaryPowerModelWrapper<TModel, TPower>
    where TModel : AbstractModel
    where TPower : PowerModel
{
    public override string CustomPackedIconPath => EnergizedSpire2PowerIconPaths.PowerImagePath(Id.Entry);

    public override string CustomBigIconPath => EnergizedSpire2PowerIconPaths.BigPowerImagePath(Id.Entry);
}