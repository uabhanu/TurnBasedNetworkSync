namespace Runtime.State
{
  using System;
  using System.Runtime.InteropServices;

  [StructLayout(LayoutKind.Sequential , Pack = 1)]
  public struct ByteBuffer96 : IEquatable<ByteBuffer96>
  {
    private const int Capacity = 96;
    private const int BitsPerByteShift = 3;
    private const int ByteOffsetMask = 7;
    private const ulong ByteMask = 0xFFUL;
    private const ulong DefaultBucketValue = 0UL;

    private ulong _b0 , _b1 , _b2 , _b3 , _b4 , _b5 , _b6 , _b7 , _b8 , _b9 , _b10 , _b11;

    public byte this[int index]
    {
      get
      {
        if((uint)index >= Capacity) throw new IndexOutOfRangeException(nameof(index));
        int bucket = index >> BitsPerByteShift;
        int shift = (index & ByteOffsetMask) << BitsPerByteShift;
        return (byte)((GetBucket(bucket) >> shift) & ByteMask);
      }
      set
      {
        if((uint)index >= Capacity) throw new IndexOutOfRangeException(nameof(index));
        int bucket = index >> BitsPerByteShift;
        int shift = (index & ByteOffsetMask) << BitsPerByteShift;
        ulong mask = ~(ByteMask << shift);
        ulong updated = (GetBucket(bucket) & mask) | ((ulong)value << shift);
        SetBucket(bucket , updated);
      }
    }

    private readonly ulong GetBucket(int bucket)
    {
      return bucket switch
      {
        0 => _b0 ,
        1 => _b1 ,
        2 => _b2 ,
        3 => _b3 ,
        4 => _b4 ,
        5 => _b5 ,
        6 => _b6 ,
        7 => _b7 ,
        8 => _b8 ,
        9 => _b9 ,
        10 => _b10 ,
        11 => _b11 ,
        _ => DefaultBucketValue
      };
    }

    private void SetBucket(int bucket , ulong value)
    {
      switch(bucket)
      {
        case 0 : _b0 = value; break;
        case 1 : _b1 = value; break;
        case 2 : _b2 = value; break;
        case 3 : _b3 = value; break;
        case 4 : _b4 = value; break;
        case 5 : _b5 = value; break;
        case 6 : _b6 = value; break;
        case 7 : _b7 = value; break;
        case 8 : _b8 = value; break;
        case 9 : _b9 = value; break;
        case 10 : _b10 = value; break;
        case 11 : _b11 = value; break;
      }
    }

    public readonly bool Equals(ByteBuffer96 other) { return _b0 == other._b0 && _b1 == other._b1 && _b2 == other._b2 && _b3 == other._b3 && _b4 == other._b4 && _b5 == other._b5 && _b6 == other._b6 && _b7 == other._b7 && _b8 == other._b8 && _b9 == other._b9 && _b10 == other._b10 && _b11 == other._b11; }

    public readonly override bool Equals(object obj) => obj is ByteBuffer96 other && Equals(other);

    public readonly override int GetHashCode()
    {
      HashCode hash = new HashCode();
      hash.Add(_b0);
      hash.Add(_b1);
      hash.Add(_b2);
      hash.Add(_b3);
      hash.Add(_b4);
      hash.Add(_b5);
      hash.Add(_b6);
      hash.Add(_b7);
      hash.Add(_b8);
      hash.Add(_b9);
      hash.Add(_b10);
      hash.Add(_b11);
      return hash.ToHashCode();
    }
  }

  [StructLayout(LayoutKind.Sequential , Pack = 1)]
  public struct ProximityGameState : IEquatable<ProximityGameState>
  {
    private const ushort InitialScore = 0;

    public const int Columns = 12;
    public const int Rows = 8;
    public const int TotalCells = Columns * Rows;

    public const byte EmptyOwner = 0;
    public const byte Player1Id = 1;
    public const byte Player2Id = 2;

    public const byte MinTileValue = 1;
    public const byte MaxTileValue = 20;
    public const byte MaxCellValue = byte.MaxValue;
    public const byte FriendlyBuffIncrement = 1;

    public uint SimulationTick;
    public byte ActivePlayerId;

    public ByteBuffer96 Owners;
    public ByteBuffer96 Values;

    public ushort ScoreP1;
    public ushort ScoreP2;

    public void RecalculateScores()
    {
      ushort s1 = InitialScore;
      ushort s2 = InitialScore;

      for(int i = 0 ; i < TotalCells ; i++)
      {
        byte owner = Owners[i];
        if(owner == Player1Id) { s1 += Values[i]; }
        else if(owner == Player2Id) { s2 += Values[i]; }
      }

      ScoreP1 = s1;
      ScoreP2 = s2;
    }

    public readonly bool Equals(ProximityGameState other) { return SimulationTick == other.SimulationTick && ActivePlayerId == other.ActivePlayerId && ScoreP1 == other.ScoreP1 && ScoreP2 == other.ScoreP2 && Owners.Equals(other.Owners) && Values.Equals(other.Values); }

    public readonly override bool Equals(object obj) => obj is ProximityGameState other && Equals(other);

    public readonly override int GetHashCode() => HashCode.Combine(SimulationTick , ActivePlayerId , ScoreP1 , ScoreP2 , Owners , Values);
  }
}