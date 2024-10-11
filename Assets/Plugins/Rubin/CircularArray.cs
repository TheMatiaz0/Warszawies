namespace Rubin
{
    public class CircularArray<T>
    {
        private T[] elements;

        public int Count => elements.Length;
        public CircularArray(int size)
        {
            elements = new T[size];
        }

        public int Wrap(int index)
        {
            return RHelper.Wrap(index,elements.Length);
        }
        public T this[int index]
        {
            get => elements[Wrap(index)];
            set => elements[Wrap(index)] = value;
        }
        
        
    }}