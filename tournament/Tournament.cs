using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
public static class Tournament
{   
    public static void Tally(Stream inStream, Stream outStream)
    {
        var matches = new List<(string teamA, string teamB, string result)>();
        using (var reader = new StreamReader(inStream))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(';');
                matches.Add((parts[0], parts[1], parts[2]));
            }
        }
        using (var writer = new StreamWriter(outStream))
        {
            writer.Write("Team                           | MP |  W |  D |  L |  P");
            var results = matches.SelectMany(m => 
            {
                if (m.result == "draw")
                {
                    return new[] {
                        (team: m.teamA, result: 'D', points: 1),
                        (team: m.teamB, result: 'D', points: 1)
                    }; 
                }
                return new[] {
                    (team: (m.result == "win" ? m.teamA : m.teamB), result: 'W', points: 3),
                    (team: (m.result == "win" ? m.teamB : m.teamA), result: 'L', points: 0)
                };
            });
            var table = results
                .GroupBy(r => r.team)
                .Select(g => new
                {
                    Name = g.Key,
                    MP = g.Count(),
                    W = g.Count(x => x.result == 'W'),
                    D = g.Count(x => x.result == 'D'),
                    L = g.Count(x => x.result == 'L'),
                    P = g.Sum(x => x.points)
                })
                .OrderByDescending(r => r.P)
                .ThenBy(r => r.Name);
            foreach(var r in table)
            {
                writer.Write($"\n{r.Name,-30} | {r.MP,2} | {r.W,2} | {r.D,2} | {r.L,2} | {r.P,2}");
            }            
        }
    }
}