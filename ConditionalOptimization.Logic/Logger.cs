using System.Collections.Concurrent;
using System.Text;

namespace ConditionalOptimization.Logic;

public class Logger : IDisposable
{
    public Logger()
    {
        _tables = new ConcurrentQueue<Table<double>>();
        _matchings = new ConcurrentQueue<IList<int>>();
        _isOn = true;
        _tablesListeningThread = new Thread(TablesListener);
        _matchingsListeningThread = new Thread(MatchingsListener);
        _tablesListeningThread.Start();
        _matchingsListeningThread.Start();
    }

    public void AddTable(Table<double> table) => _tables.Enqueue(table);
    
    public void AddMatching(IList<int> matching) => _matchings.Enqueue(matching);

    private void TablesListener()
    {
        var iteration = 0;
        while (_isOn || !_tables.IsEmpty)
        {
            if (_tables.TryDequeue(out var table))
            {
                var data = new StringBuilder($"Таблица стоимостей на {iteration} итерации:\n");

                for (var i = 0; i < table.Dimension; i++)
                {
                    for (var j = 0; j < table.Dimension; j++)
                        data.Append($"{table.Rows[i][j].Value} ");
                    data.Append('\n');
                }
                
                Console.WriteLine(data);

                iteration++;
            }
            else
                Thread.Sleep(0);
        }
    }
    
    private void MatchingsListener()
    {
        var iteration = 0;
        while (_isOn || !_matchings.IsEmpty)
        {
            if (_matchings.TryDequeue(out var matching))
            {
                var data = new StringBuilder($"Наибольшее паросочетание на {iteration} итерации:\n");
        
                for (var i = 0; i < matching.Count; i += 2) 
                    data.Append($"{matching[i]} --> {matching[i + 1]}\n");
                
                Console.WriteLine(data);

                iteration++;
            }
            else
                Thread.Sleep(0);
        }
    }

    public void Dispose()
    {
        _isOn = false;
        _tablesListeningThread.Join();
        _matchingsListeningThread.Join();
        GC.SuppressFinalize(this);
    }

    ~Logger()
    {
        _isOn = false;
        _tablesListeningThread.Join();
        _matchingsListeningThread.Join();
    }
    
    private readonly ConcurrentQueue<Table<double>> _tables;
    private readonly ConcurrentQueue<IList<int>> _matchings;
    private bool _isOn;
    private readonly Thread _tablesListeningThread;
    private readonly Thread _matchingsListeningThread;
}