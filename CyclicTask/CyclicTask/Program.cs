using System.Collections.Generic;

/*
 * Name: cyclic task 
 * введем понятие- если в связном списке есть элемент, ссылающийся на NULL, цикла нет. 
 * Если находится элемент, ссылающийся на какой-либо из предыдущих или на самого себя, то цикл есть
 * архитектор отдал интерфейс:
 * interface LLI { LLI next(); }
 * где next() возвращает следующий элемент
 * задача: написать функцию bool hasCycle(LLI lli)
 * которая принимает элемент связного списка и возвращает true если цикл в данном списке есть, и false если цикла нет
 */

/*
 * Решение задачи (олимпиадной)
 * Решение в теории мы можем сохранять ссылки на обьекты / ссылки (например на текущий) и проверять нет ли у нас совпадений
 * со следующим обьектом / ссылкой в списке (cache), если есть то мы вернулись к первому обьекту. Это может значить две вещи,
 * что у наст только один обьект и он имеет сылку на самого себя, или мы прошли все элементы в списке и его последний элемент 
 * указывал на первый элемент в списке, другими словами список закольцован или имеет цикл  
 */
namespace CyclicTask
{
    /// <summary>
    /// Interface provided by architector 
    /// </summary>
    public interface LLI
    {
        public LLI next();
    }

    /// <summary>
    /// Cyclic class is repsenting a node in list
    /// </summary>
    public interface ICyrlec : LLI
    {
        /// <summary>
        /// Check we've some node is pointed at same node or have cycled nodes
        /// </summary>
        /// <param name="lli">Our linked list</param>
        /// <returns>// if no, we have cycled nodes then true, otherwise false we haven't cycled nodes yet</returns>
        public virtual bool hasCycle(LLI lli)
        {
            var d = new HashSet<LLI>(); // HashSet vs List to avoid cyclic
            while (lli != null)
            {
                // try to add reference into HashSet to keep a list of references
                if (!d.Add(lli))
                {
                    // if no, we can't add new node then true, because some node is pointed at same node (we've same references)
                    return true;
                }

                // next node in list eg node1 (previous) => node2 (make as current)
                lli = lli.next();
            }

            // if yes, we can add new node then false, because some node isn't pointed at same node (we've not same reference in list)
            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
