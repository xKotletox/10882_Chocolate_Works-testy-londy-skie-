using ChocolateWorks.Models;
using ChocolateWorks.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ChocolateWorks.Tests;

public class CocoaProcessorTests
{
    [Test]
    public async Task ProcessCocoa_OrdersTwiceAsManyBeans()
    {
        var cocoaPlantationMock = Substitute.For<ICocoaPlantation>();
        cocoaPlantationMock.HarvestBeans(Arg.Any<decimal>()).Returns(new CocoaBeans(70m));
        ILogger<CocoaProcessor> loggerDummy = NullLogger<CocoaProcessor>.Instance;
        var processor = new CocoaProcessor(cocoaPlantationMock, loggerDummy);

        var mass = processor.ProcessCocoa(35m);

        await Assert.That(mass.Grams).IsEqualTo(35m);
        cocoaPlantationMock.Received(1).HarvestBeans(70m);
    }

    [Test]
    public async Task ProcessCocoa_ReturnsMassObtainedFromHarvestedBeans()
    {
        var cocoaPlantationStub = Substitute.For<ICocoaPlantation>();
        cocoaPlantationStub.HarvestBeans(Arg.Any<decimal>()).Returns(new CocoaBeans(100m));
        ILogger<CocoaProcessor> loggerDummy = NullLogger<CocoaProcessor>.Instance;
        var processor = new CocoaProcessor(cocoaPlantationStub, loggerDummy);

        var mass = processor.ProcessCocoa(50m);

        await Assert.That(mass.Grams).IsEqualTo(50m);
    }

    [Test]
    [Arguments(0)]
    [Arguments(-10)]
    public async Task ProcessCocoa_NonPositiveAmount_ThrowsAndHarvestsNothing(int grams)
    {
        var cocoaPlantationMock = Substitute.For<ICocoaPlantation>();
        ILogger<CocoaProcessor> loggerDummy = NullLogger<CocoaProcessor>.Instance;
        var processor = new CocoaProcessor(cocoaPlantationMock, loggerDummy);

        await Assert.That(() => processor.ProcessCocoa(grams))
            .Throws<ArgumentOutOfRangeException>();

        cocoaPlantationMock.DidNotReceiveWithAnyArgs().HarvestBeans(default);
    }
}

public class SugarRefineryTests
{
    [Test]
    public async Task RefineSugar_OrdersSixTimesMoreBeets()
    {
        var sugarBeetFarmMock = Substitute.For<ISugarBeetFarm>();
        sugarBeetFarmMock.HarvestBeets(Arg.Any<decimal>()).Returns(new SugarBeets(240m));
        ILogger<SugarRefinery> loggerDummy = NullLogger<SugarRefinery>.Instance;
        var refinery = new SugarRefinery(sugarBeetFarmMock, loggerDummy);

        var sugar = refinery.RefineSugar(40m);

        await Assert.That(sugar.Grams).IsEqualTo(40m);
        sugarBeetFarmMock.Received(1).HarvestBeets(240m);
    }

    [Test]
    public async Task RefineSugar_ReturnsSugarObtainedFromHarvestedBeets()
    {
        var sugarBeetFarmStub = Substitute.For<ISugarBeetFarm>();
        sugarBeetFarmStub.HarvestBeets(Arg.Any<decimal>()).Returns(new SugarBeets(60m));
        ILogger<SugarRefinery> loggerDummy = NullLogger<SugarRefinery>.Instance;
        var refinery = new SugarRefinery(sugarBeetFarmStub, loggerDummy);

        var sugar = refinery.RefineSugar(10m);

        await Assert.That(sugar.Grams).IsEqualTo(10m);
    }

    [Test]
    [Arguments(0)]
    [Arguments(-5)]
    public async Task RefineSugar_NonPositiveAmount_ThrowsAndHarvestsNothing(int grams)
    {
        var sugarBeetFarmMock = Substitute.For<ISugarBeetFarm>();
        ILogger<SugarRefinery> loggerDummy = NullLogger<SugarRefinery>.Instance;
        var refinery = new SugarRefinery(sugarBeetFarmMock, loggerDummy);

        await Assert.That(() => refinery.RefineSugar(grams))
            .Throws<ArgumentOutOfRangeException>();

        sugarBeetFarmMock.DidNotReceiveWithAnyArgs().HarvestBeets(default);
    }
}

public class LabelPrinterTests
{
    [Test]
    public async Task PrintLabel_CalledTwice_ReturnsConsecutiveLabels()
    {
        ILogger<LabelPrinter> loggerDummy = NullLogger<LabelPrinter>.Instance;
        var printer = new LabelPrinter(loggerDummy);

        var first = printer.PrintLabel(ChocolateType.Dark, 100);
        var second = printer.PrintLabel(ChocolateType.Milk, 200);

        await Assert.That(first).IsEqualTo("DARK-100G-0001");
        await Assert.That(second).IsEqualTo("MILK-200G-0002");
    }
}