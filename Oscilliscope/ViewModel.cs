using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace Oscilliscope;

public class ViewModel
{
    public ISeries[] Series { get; set; }
        = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = new double[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 },
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0
            }
        };
}
