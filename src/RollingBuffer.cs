using System.Numerics;

public class Accumulator<T> where T : INumber<T> {

    public T Get => Total();
    public T Average => Total() / countNumber;



    private int count;
    private T countNumber;
    private T zero;



    private int index;
    private T[] values;



    public Accumulator(int count) {
        this.count = count;
        countNumber = T.CreateChecked(count);
        zero = T.CreateChecked(0);
        values = new T[count];
    }

    public void Add(T item) {
        values[index] = item;
        index = (index + 1) % count;
    }

    private T Total() {
        T total = zero;
        for (int i = 0; i < values.Length; i++) {
            total += values[i];
        }
        return total;
    }
}