using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace Oscilliscope;

public partial class ViewModel : ObservableObject
{
    private static ObservableCollection<ObservableValue> observableValues;

    public ViewModel()
    {
        observableValues = new ObservableCollection<ObservableValue>
        {
        new ObservableValue(2),
        new ObservableValue(3),
        new ObservableValue(4),
        };
    }

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<ObservableValue>
        {
            Values = observableValues,
            Fill = null,
            GeometrySize = 0,
            LineSmoothness = 0
        }
    };
}

