using ChocolateWorks.Models;
using ChocolateWorks.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ChocolateWorks.Tests;

public class ChocolateFactoryTests
{
    [Test]
    public async Task ProduceBar_MilkChocolate_ReturnsBarMadeFromSuppliedIngredients()
    {
        var cocoaProcessorStub = Substitute.For<ICocoaProcessor>();
        cocoaProcessorStub.ProcessCocoa(Arg.Any<decimal>()).Returns(new CocoaMass(35m));
        var sugarRefineryStub = Substitute.For<ISugarRefinery>();
        sugarRefineryStub.RefineSugar(Arg.Any<decimal>()).Returns(new Sugar(40m));
        var dairyStub = Substitute.For<IDairy>();
        dairyStub.ProduceMilkPowder(Arg.Any<decimal>()).Returns(new MilkPowder(25m));
        var labelPrinterStub = Substitute.For<ILabelPrinter>();
        labelPrinterStub.PrintLabel(ChocolateType.Milk, 100).Returns("MILK-100G-TEST");
        ILogger<ChocolateFactory> loggerDummy = NullLogger<ChocolateFactory>.Instance;

        var factory = new ChocolateFactory(cocoaProcessorStub, sugarRefineryStub,
            dairyStub, labelPrinterStub, loggerDummy);

        var bar = factory.ProduceBar(new ChocolateOrder(ChocolateType.Milk, 100));

        await Assert.That(bar.Label).IsEqualTo("MILK-100G-TEST");
        await Assert.That(bar.Type).IsEqualTo(ChocolateType.Milk);
        await Assert.That(bar.WeightGrams).IsEqualTo(100);
        await Assert.That(bar.CocoaGrams).IsEqualTo(35m);
        await Assert.That(bar.SugarGrams).IsEqualTo(40m);
        await Assert.That(bar.MilkGrams).IsEqualTo(25m);
    }

    [Test]
    [Arguments(ChocolateType.Dark, 70)]
    [Arguments(ChocolateType.Milk, 35)]
    [Arguments(ChocolateType.White, 30)]
    public async Task ProduceBar_OrdersCocoaAccordingToRecipe(ChocolateType type, int expectedGrams)
    {
        var cocoaProcessorMock = Substitute.For<ICocoaProcessor>();
        cocoaProcessorMock.ProcessCocoa(Arg.Any<decimal>()).Returns(new CocoaMass(expectedGrams));
        var sugarRefineryStub = Substitute.For<ISugarRefinery>();
        sugarRefineryStub.RefineSugar(Arg.Any<decimal>()).Returns(new Sugar(1m));
        var dairyStub = Substitute.For<IDairy>();
        dairyStub.ProduceMilkPowder(Arg.Any<decimal>()).Returns(new MilkPowder(1m));
        var labelPrinterStub = Substitute.For<ILabelPrinter>();
        labelPrinterStub.PrintLabel(Arg.Any<ChocolateType>(), Arg.Any<int>()).Returns("TEST");
        ILogger<ChocolateFactory> loggerDummy = NullLogger<ChocolateFactory>.Instance;

        var factory = new ChocolateFactory(cocoaProcessorMock, sugarRefineryStub,
            dairyStub, labelPrinterStub, loggerDummy);

        var bar = factory.ProduceBar(new ChocolateOrder(type, 100));

        await Assert.That(bar.CocoaGrams).IsEqualTo((decimal)expectedGrams);
        cocoaProcessorMock.Received(1).ProcessCocoa(expectedGrams);
    }

    [Test]
    [Arguments(ChocolateType.Dark, 30)]
    [Arguments(ChocolateType.Milk, 40)]
    [Arguments(ChocolateType.White, 45)]
    public async Task ProduceBar_OrdersSugarAccordingToRecipe(ChocolateType type, int expectedGrams)
    {
        var cocoaProcessorStub = Substitute.For<ICocoaProcessor>();
        cocoaProcessorStub.ProcessCocoa(Arg.Any<decimal>()).Returns(new CocoaMass(1m));
        var sugarRefineryMock = Substitute.For<ISugarRefinery>();
        sugarRefineryMock.RefineSugar(Arg.Any<decimal>()).Returns(new Sugar(expectedGrams));
        var dairyStub = Substitute.For<IDairy>();
        dairyStub.ProduceMilkPowder(Arg.Any<decimal>()).Returns(new MilkPowder(1m));
        var labelPrinterStub = Substitute.For<ILabelPrinter>();
        labelPrinterStub.PrintLabel(Arg.Any<ChocolateType>(), Arg.Any<int>()).Returns("TEST");
        ILogger<ChocolateFactory> loggerDummy = NullLogger<ChocolateFactory>.Instance;

        var factory = new ChocolateFactory(cocoaProcessorStub, sugarRefineryMock,
            dairyStub, labelPrinterStub, loggerDummy);

        var bar = factory.ProduceBar(new ChocolateOrder(type, 100));

        await Assert.That(bar.SugarGrams).IsEqualTo((decimal)expectedGrams);
        sugarRefineryMock.Received(1).RefineSugar(expectedGrams);
    }

    [Test]
    [Arguments(ChocolateType.Milk)]
    [Arguments(ChocolateType.White)]
    public async Task ProduceBar_ChocolateWithMilk_OrdersMilkPowder(ChocolateType type)
    {
        var cocoaProcessorStub = Substitute.For<ICocoaProcessor>();
        cocoaProcessorStub.ProcessCocoa(Arg.Any<decimal>()).Returns(new CocoaMass(1m));
        var sugarRefineryStub = Substitute.For<ISugarRefinery>();
        sugarRefineryStub.RefineSugar(Arg.Any<decimal>()).Returns(new Sugar(1m));
        var dairyMock = Substitute.For<IDairy>();
        dairyMock.ProduceMilkPowder(Arg.Any<decimal>()).Returns(new MilkPowder(50m));
        var labelPrinterStub = Substitute.For<ILabelPrinter>();
        labelPrinterStub.PrintLabel(Arg.Any<ChocolateType>(), Arg.Any<int>()).Returns("TEST");
        ILogger<ChocolateFactory> loggerDummy = NullLogger<ChocolateFactory>.Instance;

        var factory = new ChocolateFactory(cocoaProcessorStub, sugarRefineryStub,
            dairyMock, labelPrinterStub, loggerDummy);

        var bar = factory.ProduceBar(new ChocolateOrder(type, 200));

        await Assert.That(bar.MilkGrams).IsEqualTo(50m);
        dairyMock.Received(1).ProduceMilkPowder(50m);
    }

    [Test]
    public async Task ProduceBar_DarkChocolate_DoesNotOrderMilk()
    {
        var cocoaProcessorStub = Substitute.For<ICocoaProcessor>();
        cocoaProcessorStub.ProcessCocoa(Arg.Any<decimal>()).Returns(new CocoaMass(70m));
        var sugarRefineryStub = Substitute.For<ISugarRefinery>();
        sugarRefineryStub.RefineSugar(Arg.Any<decimal>()).Returns(new Sugar(30m));
        var dairyMock = Substitute.For<IDairy>();
        var labelPrinterStub = Substitute.For<ILabelPrinter>();
        labelPrinterStub.PrintLabel(Arg.Any<ChocolateType>(), Arg.Any<int>()).Returns("TEST");
        ILogger<ChocolateFactory> loggerDummy = NullLogger<ChocolateFactory>.Instance;

        var factory = new ChocolateFactory(cocoaProcessorStub, sugarRefineryStub,
            dairyMock, labelPrinterStub, loggerDummy);

        var bar = factory.ProduceBar(new ChocolateOrder(ChocolateType.Dark, 100));

        await Assert.That(bar.MilkGrams).IsEqualTo(0m);
        dairyMock.DidNotReceiveWithAnyArgs().ProduceMilkPowder(default);
    }

    [Test]
    public async Task ProduceBar_ValidOrder_PrintsExactlyOneLabel()
    {
        var cocoaProcessorStub = Substitute.For<ICocoaProcessor>();
        cocoaProcessorStub.ProcessCocoa(Arg.Any<decimal>()).Returns(new CocoaMass(1m));
        var sugarRefineryStub = Substitute.For<ISugarRefinery>();
        sugarRefineryStub.RefineSugar(Arg.Any<decimal>()).Returns(new Sugar(1m));
        var dairyStub = Substitute.For<IDairy>();
        dairyStub.ProduceMilkPowder(Arg.Any<decimal>()).Returns(new MilkPowder(1m));
        var labelPrinterMock = Substitute.For<ILabelPrinter>();
        labelPrinterMock.PrintLabel(ChocolateType.White, 250).Returns("WHITE-250G-0042");
        ILogger<ChocolateFactory> loggerDummy = NullLogger<ChocolateFactory>.Instance;

        var factory = new ChocolateFactory(cocoaProcessorStub, sugarRefineryStub,
            dairyStub, labelPrinterMock, loggerDummy);

        var bar = factory.ProduceBar(new ChocolateOrder(ChocolateType.White, 250));

        await Assert.That(bar.Label).IsEqualTo("WHITE-250G-0042");
        labelPrinterMock.Received(1).PrintLabel(ChocolateType.White, 250);
    }

    [Test]
    [Arguments(0)]
    [Arguments(24)]
    [Arguments(501)]
    public async Task ProduceBar_WeightOutOfRange_ThrowsAndOrdersNothing(int weight)
    {
        var cocoaProcessorMock = Substitute.For<ICocoaProcessor>();
        var sugarRefineryMock = Substitute.For<ISugarRefinery>();
        var dairyMock = Substitute.For<IDairy>();
        var labelPrinterMock = Substitute.For<ILabelPrinter>();
        ILogger<ChocolateFactory> loggerDummy = NullLogger<ChocolateFactory>.Instance;

        var factory = new ChocolateFactory(cocoaProcessorMock, sugarRefineryMock,
            dairyMock, labelPrinterMock, loggerDummy);

        await Assert.That(() => factory.ProduceBar(new ChocolateOrder(ChocolateType.Milk, weight)))
            .Throws<ArgumentOutOfRangeException>();

        cocoaProcessorMock.DidNotReceiveWithAnyArgs().ProcessCocoa(default);
        sugarRefineryMock.DidNotReceiveWithAnyArgs().RefineSugar(default);
        dairyMock.DidNotReceiveWithAnyArgs().ProduceMilkPowder(default);
        labelPrinterMock.DidNotReceiveWithAnyArgs().PrintLabel(default, default);
    }
}