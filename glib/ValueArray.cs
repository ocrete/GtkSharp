// ValueArray.cs - ValueArray wrapper implementation
//
// Authors: Mike Kestner <mkestner@ximian.com>
//
// (c) 2003 Novell, Inc.

namespace GLib {

	using System;
	using System.Collections;
	using System.Runtime.InteropServices;

	public class ValueArray : IDisposable, ICollection, ICloneable, IWrapper {

		private IntPtr handle = IntPtr.Zero;

		[DllImport("libgobject-2.0-0.dll")]
		static extern IntPtr g_value_array_new (uint n_preallocs);

		public ValueArray (uint n_preallocs)
		{
			handle = g_value_array_new (n_preallocs);
		}

		internal ValueArray (IntPtr raw)
		{
			handle = raw;
		}
		
		~ValueArray ()
		{
			Dispose (false);
		}
		
		// IDisposable
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		[DllImport("libgobject-2.0-0.dll")]
		static extern void g_value_array_free (IntPtr raw);

		void Dispose (bool disposing)
		{
			if (Handle == IntPtr.Zero)
				return;

			g_value_array_free (Handle);
			handle = IntPtr.Zero;
		}
		
		public IntPtr Handle {
			get {
				return handle;
			}
		}

		[DllImport("gtksharpglue")]
		static extern IntPtr gtksharp_value_array_get_array (IntPtr raw);

		public IntPtr ArrayPtr {
			get {
				return gtksharp_value_array_get_array (Handle);
			}
		}

		[DllImport("libgobject-2.0-0.dll")]
		static extern void g_value_array_append (IntPtr raw, IntPtr val);

		public void Append (GLib.Value val)
		{
			g_value_array_append (Handle, val.Handle);
		}

		[DllImport("libgobject-2.0-0.dll")]
		static extern void g_value_array_insert (IntPtr raw, uint idx, IntPtr val);

		public void Insert (uint idx, GLib.Value val)
		{
			g_value_array_insert (Handle, idx, val.Handle);
		}

		[DllImport("libgobject-2.0-0.dll")]
		static extern void g_value_array_prepend (IntPtr raw, IntPtr val);

		public void Prepend (GLib.Value val)
		{
			g_value_array_prepend (Handle, val.Handle);
		}

		[DllImport("libgobject-2.0-0.dll")]
		static extern void g_value_array_remove (IntPtr raw, uint idx);

		public void Remove (uint idx)
		{
			g_value_array_remove (Handle, idx);
		}

		[DllImport("gtksharpglue")]
		static extern int gtksharp_value_array_get_count (IntPtr raw);

		// ICollection
		public int Count {
			get {
				return gtksharp_value_array_get_count (Handle);
			}
		}

		[DllImport("libgobject-2.0-0.dll")]
		static extern IntPtr g_value_array_get_nth (IntPtr raw, uint idx);

		public object this [int index] { 
			get { 
				return new GLib.Value (g_value_array_get_nth (Handle, (uint) index), IntPtr.Zero);
			}
		}

		// Synchronization could be tricky here. Hmm.
		public bool IsSynchronized {
			get { return false; }
		}

		public object SyncRoot {
			get { return null; }
		}

		public void CopyTo (Array array, int index)
		{
			if (array == null)
				throw new ArgumentNullException ("Array can't be null.");

			if (index < 0)
				throw new ArgumentOutOfRangeException ("Index must be greater than 0.");

			if (index + Count < array.Length)
				throw new ArgumentException ("Array not large enough to copy into starting at index.");
			
			for (int i = 0; i < Count; i++)
				((IList) array) [index + i] = this [i];
		}

		private class ListEnumerator : IEnumerator
		{
			private int current = -1;
			private ValueArray vals;

			public ListEnumerator (ValueArray vals)
			{
				this.vals = vals;
			}

			public object Current {
				get {
					if (current == -1)
						return null;
					return vals [current];
				}
			}

			public bool MoveNext ()
			{
				if (++current >= vals.Count) {
					current = -1;
					return false;
				}

				return true;
			}

			public void Reset ()
			{
				current = -1;
			}
		}
		
		// IEnumerable
		public IEnumerator GetEnumerator ()
		{
			return new ListEnumerator (this);
		}

		[DllImport("libgobject-2.0-0.dll")]
		static extern IntPtr g_value_array_copy (IntPtr raw);

		// ICloneable
		public object Clone ()
		{
			return new ValueArray (g_value_array_copy (Handle));
		}
	}
}