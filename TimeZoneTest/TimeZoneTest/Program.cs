using System;

using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using TimeZoneTest.Adapter; 

namespace TimeZoneTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Conc().GlobalAdd();
            
            var sem = new Sem(); // heap
            sem.Count();
            Console.ReadLine();
            
            var strs = new string[] { "compilations", "bewailed", "horology", "lactated", "blindsided", "swoop", "foretasted", "ware", "abuts", "stepchild", "arriving", "magnet", "vacating", "relegates", "scale", "melodically", "proprietresses", "parties", "ambiguities", "bootblacks", "shipbuilders", "umping", "belittling", "lefty", "foremost", "bifocals", "moorish", "temblors", "edited", "hint", "serenest", "rendezvousing", "schoolmate", "fertilizers", "daiquiri", "starr", "federate", "rectal", "case", "kielbasas", "monogamous", "inflectional", "zapata", "permitted", "concessions", "easters", "communique", "angelica", "shepherdess", "jaundiced", "breaks", "raspy", "harpooned", "innocence", "craters", "cajun", "pueblos", "housetop", "traits", "bluejacket", "pete", "snots", "wagging", "tangling", "cheesecakes", "constructing", "balanchine", "paralyzed", "aftereffects", "dotingly", "definitions", "renovations", "surfboards", "lifework", "knacking", "apprises", "minimalism", "skyrocketed", "artworks", "instrumentals", "eardrums", "hunching", "codification", "vainglory", "clarendon", "peters", "weeknight", "statistics", "ay", "aureomycin", "lorrie", "compassed", "speccing", "galen", "concerto", "rocky", "derision", "exonerate", "sultrier", "mastoids", "repackage", "cyclical", "gowns", "regionalism", "supplementary", "bierce", "darby", "memorize", "songster", "biplane", "calibrates", "decriminalizes", "shack", "idleness", "confessions", "snippy", "barometer", "earthing", "sequence", "hastiness", "emitted", "superintends", "stockades", "busywork", "dvina", "aggravated", "furbelow", "hashish", "overextended", "foreordain", "lie", "insurance", "recollected", "interpreted", "congregate", "ranks", "juts", "dampen", "gaits", "eroticism", "neighborhoods", "perihelion", "simulations", "fumigating", "balkiest", "semite", "epicure", "heavier", "masterpiece", "bettering", "lizzie", "wail", "batsmen", "unbolt", "cudgeling", "bungalow", "behalves", "refurnishes", "pram", "spoonerisms", "cornered", "rises", "encroachments", "gabon", "cultivation", "parsed", "takeovers", "stampeded", "persia", "devotional", "doorbells", "psalms", "cains", "copulated", "archetypal", "cursores", "inbred", "paradigmatic", "thesauri", "rose", "stopcocks", "weakness", "ballsier", "jagiellon", "torches", "hover", "conservationists", "brightening", "dotted", "rodgers", "mandalay", "overjoying", "supervision", "gonads", "portage", "crap", "capers", "posy", "collateral", "funny", "garvey", "ravenously", "arias", "kirghiz", "elton", "gambolled", "highboy", "kneecaps", "southey", "etymology", "overeager", "numbers", "ebullience", "unseemly", "airbrushes", "excruciating", "gemstones", "juiciest", "muftis", "shadowing", "organically", "plume", "guppy", "obscurely", "clinker", "confederacies", "unhurried", "monastic", "witty", "breastbones", "ijsselmeer", "dublin", "linnaeus", "dervish", "bluefish", "selectric", "syllable", "pogroms", "pacesetters", "anastasia", "pandora", "foci", "bipartisan", "loomed", "emits", "gracious", "warfare", "uncouples", "augusts", "portray", "refinery", "resonances", "expediters", "deputations", "indubitably", "richly", "motivational", "gringo", "hubris", "mislay", "scad", "lambastes", "reemerged", "wart", "zirconium", "linus", "moussorgsky", "swopped", "sufferer", "sputtered", "tamed", "merrimack", "conglomerate", "blaspheme", "overcompensate", "rheas", "pares", "ranted", "prisoning", "rumor", "gabbles", "lummox", "lactated", "unzipping", "tirelessly", "backdate", "puzzling", "interject", "rejections", "bust", "centered", "oxymoron", "tangibles", "sejong", "not", "tameness", "consumings", "prostrated", "rowdyism", "ardent", "macabre", "rustics", "dodoes", "warheads", "wraths", "bournemouth", "staffers", "retold", "stiflings", "petrifaction", "larkspurs", "crunching", "clanks", "briefest", "clinches", "attaching", "extinguished", "ryder", "shiny", "antiqued", "gags", "assessments", "simulated", "dialed", "confesses", "livelongs", "dimensions", "lodgings", "cormorants", "canaries", "spineless", "widening", "chappaquiddick", "blurry", "lassa", "vilyui", "desertions", "trinket", "teamed", "bidets", "mods", "lessors", "impressiveness", "subjugated", "rumpuses", "swamies", "annotations", "batiks", "ratliff", "waxwork", "grander", "junta", "chutney", "exalted", "yawl", "joke", "vocational", "diabetic", "bullying", "edit", "losing", "banns", "doleful", "precision", "excreting", "foals", "smarten", "soliciting", "disturbance", "soggily", "gabrielle", "margret", "faded", "pane", "jerusalem", "bedpan", "overtaxed", "brigs", "honors", "repackage", "croissants", "kirov", "crummier", "limeades", "grandson", "criers", "bring", "jaundicing", "omnibusses", "gawking", "tonsillectomies", "deodorizer", "nosedove", "commence", "faulkner", "adultery", "shakedown", "wigwag", "wiper", "compatible", "ultra", "adamant", "distillation", "gestates", "semi", "inmate", "onlookers", "grudgingly", "recipe", "chaise", "dialectal", "aphids", "flimsier", "orgasm", "sobs", "swellheaded", "utilize", "karenina", "irreparably", "preteen", "mumble", "gingersnaps", "alumnus", "chummiest", "snobbish", "crawlspaces", "inappropriate", "ought", "continence", "hydrogenate", "eskimo", "desolated", "oceanic", "evasive", "sake", "laziest", "tramps", "joyridden", "acclimatized", "riffraff", "thanklessly", "harmonizing", "guinevere", "demanded", "capabler", "syphilitics", "brainteaser", "creamers", "upholds", "stiflings", "walt", "luau", "deafen", "concretely", "unhand", "animations", "map", "limbos", "tranquil", "windbreakers", "limoges", "varying", "declensions", "signs", "green", "snowbelt", "homosexual", "hopping", "residue", "ransacked", "emeritus", "pathologist", "brazenly", "forbiddingly", "alfredo", "glummest", "deciphered", "delusive", "repentant", "complainants", "beets", "syntactics", "vicissitude", "incompetents", "concur", "canaan", "rowdies", "streamer", "martinets", "shapeliness", "videodiscs", "restfulness", "rhea", "consumed", "pooching", "disenfranchisement", "impoverishes", "behalf", "unsuccessfully", "complicity", "ulcerating", "derisive", "jephthah", "clearing", "reputation", "kansan", "sledgehammer", "benchmarks", "escutcheon", "portfolios", "mandolins", "marketable", "megalomaniacs", "kinking", "bombarding", "wimple", "perishes", "rukeyser", "squatter", "coddle", "traditionalists", "sifts", "agglomerations", "seasonings", "brightness", "spices", "claimant", "sofas", "ambulatories", "bothered", "businessmen", "orly", "kinetic", "contracted", "grenadiers", "flooding", "dissolved", "corroboration", "mussed", "squareness", "alabamans", "dandelions", "labyrinthine", "pot", "waxwing", "residential", "pizza", "overjoying", "whelps", "overlaying", "elanor", "tented", "masterminded", "balsamed", "powerhouses", "tramps", "eisenstein", "voile", "repellents", "beaus", "coordinated", "wreckers", "eternities", "untwists", "estrangements", "vitreous", "embodied" };
            var strs1 = new string[] { "", "" };
            var strs2 = new string[] { "" };
            Console.WriteLine(Solution.GroupAnagrams3(strs));
            Console.ReadLine();
            

            var timer = new Stopwatch();
            int n = 40;
            
            // прогрев
            timer.Restart();
            var pr = Process.GetCurrentProcess();
            var mem0 = pr.PeakVirtualMemorySize64;
            Console.WriteLine(Fibonacci.FibonacciRec(n));
            var memuse = pr.PeakVirtualMemorySize64 - mem0;
            Console.WriteLine(memuse);
            timer.Stop();
            Console.WriteLine(timer.ElapsedMilliseconds);

            //--------------
            Console.WriteLine("Rec");
            timer.Restart();
            Console.WriteLine(Fibonacci.FibonacciRec(n));
            timer.Stop();
            Console.WriteLine("Tick - " + timer.ElapsedTicks);
            Console.WriteLine("Secs - " + timer.ElapsedMilliseconds);
            
            Console.WriteLine("Cache");
            timer.Restart();
            Console.WriteLine(Fibonacci.FibonacciCache(n, new int[n]));
            timer.Stop();
            Console.WriteLine("Tick - " + timer.ElapsedTicks);
            Console.WriteLine("Secs - " + timer.ElapsedMilliseconds);
            
            Console.WriteLine("Lin");
            timer.Restart();
            Console.WriteLine(Fibonacci.FibonacciLin(n));
            timer.Stop();
            Console.WriteLine("Tick - " + timer.ElapsedTicks);
            Console.WriteLine("Secs - " + timer.ElapsedMilliseconds);

            Console.ReadLine();

            // сложность, фибоначчи - сложность функци!
            // хранимая процедура, функции (текстовой)!
            System.Collections.Concurrent.ConcurrentDictionary<string, string> d;
            System.Collections.Generic.Dictionary<string, string> dd;

            var f2 = new X { sum = 0 };

            var t3 = Y.Sum(f2);
            var t4 = Y.Sum(f2);

            var results = Task.WhenAll(t3, t4);

            results.Wait();

            Console.WriteLine(f2.sum);

            Console.ReadLine();


            var x = 3;
            var y = 3;

            var sw = Stopwatch.StartNew();
            var robot = Robot.GetPathF(x, y);
            sw.Stop();
            Console.WriteLine(robot);
            Console.WriteLine(sw.ElapsedMilliseconds);

            var swM = Stopwatch.StartNew();
            var robotM = Robot.GetPathFMemoize(x, y);
            swM.Stop();
            Console.WriteLine(robotM);
            Console.WriteLine(swM.ElapsedMilliseconds);

            var swL = Stopwatch.StartNew();
            var robotL = Robot.GetPathFMemoizeLazy(x, y);
            swL.Stop();
            Console.WriteLine(robotL);
            Console.WriteLine(swL.ElapsedMilliseconds);

            Console.ReadLine();
        }
        struct S { }
        public static Action<X> A = (p) =>
        {
            lock (p) // Monitor
            {
                for (int i = 0; i < 1000000; i++)
                {
                    p.sum++;
                }
            }
            return;
        };
    }

    public class X
    {
        public int sum { get; set; }
    }


    //---------------
    public class Y
    { 
        public static async Task<int> Sum(X x)
        {
            await Task.Delay(100);

            lock (x)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    x.sum++;
                }
            }

            await Task.Delay(100);

            return x.sum;
        }
    }
}
