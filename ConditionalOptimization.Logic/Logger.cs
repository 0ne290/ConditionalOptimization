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
        while (_isOn || !_tables.IsEmpty)
        {
            if (_tables.TryDequeue(out var table))
            {
                var data = new StringBuilder();

                for (var i = 0; i < table.Dimension; i++)
                {
                    for (var j = 0; j < table.Dimension; j++)
                        data.Append($"{table.Rows[i][j].Value} ");
                    data.Append('\n');
                }
                
                Console.WriteLine(data);
            }
            else
                Thread.Sleep(0);
        }
    }
    
    private void MatchingsListener()
    {
        while (_isOn || !_matchings.IsEmpty)
        {
            if (_matchings.TryDequeue(out var matching))
            {
                var data = new StringBuilder();
        
                foreach (var node in matching) 
                    data.Append($"{node} -->\n");
                
                Console.WriteLine(data);
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