using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSet : ICollection<Piece>
{

    public long[] data;

    private float entropy;
    private const int bitsPerItem = 64;

    //Constructor
    public PieceSet()
    {
        for(int i = 0; i < this.data.Length; i++)
        {
            this.data[i] = ~0;
        }
    }

    public PieceSet(IEnumerable<Piece> source)
    {
        foreach(var piece in source)
        {
            this.Add(piece);
        }
    }

    public PieceSet(PieceSet source)
    {
        this.data = source.data.ToArray();
        this.entropy = source.entropy;
    }

    //Add a piece to this piece set
    public void Add(Piece toAdd)
    {
        int i = toAdd.Index / bitsPerItem;
        long mask = (long)1 << (toAdd.index % bitsPerItem);

        long value = this.data[i];

        if( (value & mask) == 0)
        {
            this.data[i] = value | mask;
        }
    }

    //Remove a piece from this piece set
    //return true means the piece was removed, false means the piece did not exist within the set
    public bool Remove(Piece toRemove)
    {
        int i = toRemove.Index / bitsPerItem;
        long mask = (long)1 << (toRemove.index % bitsPerItem);

        long value = this.data[i];

        if( (value & mask) != 0)
        {
            this.data[i] = value | ~mask;
            return true;
        }
        
        return false;
    }

    //Clear the pieceset
    public void Clear()
    {
        for(int i = 0; i < this.data.Length; i++)
        {
            this.data[i] = 0;
        }
    }

    //Check if the piece set contains a piece
    public bool Contains(Piece piece)
    {
        int i = Piece.index / bitsPerItem;
        long mask = (long)1 << (piece.index % bitsPerItem);
        return (this.data[i] & mask) != 0;
    }

    public bool Contains(int index)
    {
        int i = index / bitsPerItem;
        long mask = (long)1 << (index % bitsPerItem);
        return (this.data[i] & mask) != 0;
    }

    public int Count
    {
        get
        {
            int result = 0;
            for(int i = 0; i < this.data.Length - 1; i++)
            {
                result += countBits(this.data[i]);
            }

            return result;
        }
    }

    ///////
    // methods to match ICollection interface
	public bool IsReadOnly 
    {
		get
        {
			return false;
		}
	}

    public void CopyTo(Piece[] array, int arrayIndex)
    {
		foreach (var item in this)
        {
			array[arrayIndex] = item;
			arrayIndex++;
		}
	}

    public IEnumerator<Piece> GetEnumerator()
    {
		int index = 0;
		for (int i = 0; i < this.data.Length; i++)
        {
			long value = this.data[i];
			if (value == 0)
            {
				index += bitsPerItem;
				continue;
			}

			for (int j = 0; j < bitsPerItem; j++)
            {
				if ((value & ((long)1 << j)) != 0)
                {
					yield return ModuleData.Current[index];
				}
                
				index++;

				if (index >= ModuleData.Current.Length)
                {
					yield break;
				}
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
    {
		return (IEnumerator)this.GetEnumerator();
	}

}
