using System;
using NPack;
using NPack.Interfaces;

namespace GravurGIS.Rendering
{
    /// <summary>
    /// Represents a column-major convention affine transform matrix.
    /// </summary>
    public class AffineMatrix : Matrix, 
                                IAffineTransformMatrix<DoubleComponent, Vector, AffineMatrix>,
                                IAffineTransformMatrix<DoubleComponent>
    {
        private static readonly DoubleComponent _zero = new DoubleComponent(0);
        private static readonly DoubleComponent _one = new DoubleComponent(1);

        /// <summary>
        /// Creates a new identiy affine transform matrix (with default(T).One in each element of the diagonal)
        /// with the given <paramref name="rank"/> for the number of rows and number of columns.
        /// </summary>
        /// <param name="format">Storage format of the matrix, either row-major or column-major.</param>
        /// <param name="rank">Number of rows and columns in the affine matrix.</param>
        /// <exception cref="NotSupportedException">If the matrix has a rank other than 3 or 4.</exception>
        public AffineMatrix(MatrixFormat format, Int32 rank)
            : base(format, rank, rank)
        {
            for (Int32 i = 0; i < rank; i++)
            {
                this[i, i] = _one;
            }
        }

        /// <summary>
        /// Creates a new affine transform matrix with the given <paramref name="elements"/>.
        /// </summary>
        /// <param name="format">Storage format of the matrix, either row-major or column-major.</param>
        /// <param name="elements">
        /// The elements for the array, with rows in the outer array for a <see cref="MatrixFormat.RowMajor"/> 
        /// matrix, and columns in the outer array for a <see cref="MatrixFormat.ColumnMajor"/> matrix.
        /// </param>
        /// <exception cref="ArgumentException">If the matrix is not square.</exception>
        /// <exception cref="NotSupportedException">If the matrix has a rank other than 3 or 4.</exception>
        public AffineMatrix(MatrixFormat format, DoubleComponent[][] elements)
            : base(format, elements)
        {
            checkSquare(elements.Length, elements[0].Length);
        }

        /// <summary>
        /// Creates a new affine transform matrix from the given <paramref name="matrix"/>.
        /// </summary>
        /// <param name="matrix">The matrix to copy.</param>
        /// <exception cref="ArgumentException">If the matrix is not square.</exception>
        /// <exception cref="NotSupportedException">If the matrix has a rank other than 3 or 4.</exception>
        public AffineMatrix(IMatrix<DoubleComponent> matrix)
            : base(matrix)
        {
            checkSquare(matrix.RowCount, matrix.ColumnCount);
        }

        public override void Scale(DoubleComponent amount, MatrixOperationOrder order)
        {
            Vector scaleVector = new Vector(RowCount - 1);

            for (Int32 i = 0; i < scaleVector.ComponentCount; i++)
            {
                scaleVector[i] = amount;
            }

            Scale(scaleVector, MatrixOperationOrder.Default);
        }

        #region IAffineTransformMatrix<T> Members

        /// <summary>
        /// Gets the inverse of the <see cref="AffineMatrix"/>.
        /// </summary>
        public new AffineMatrix Inverse
        {
            get { return new AffineMatrix(base.Inverse); }
        }

        /// <summary>
        /// Resets the affine transform to the identity matrix (a diagonal of one).
        /// </summary>
        public void Reset()
        {
            for (Int32 i = 0; i < RowCount; i++)
            {
                for (Int32 j = 0; j < ColumnCount; j++)
                {
                    this[i, j] = i == j ? _one : _zero;
                }
            }
        }

        /// <summary>
        /// Rotates the affine transform around the given <paramref name="axis"/> at the given <paramref name="point"/>.
        /// </summary>
        /// <param name="point">Point at which to compute the rotation.</param>
        /// <param name="axis">The axis to rotate around. May be an addition of the basis vectors.</param>
        /// <param name="radians">Angle to rotate through.</param>
        public void RotateAt(Vector point, Vector axis, Double radians)
        {
            if (Format == MatrixFormat.RowMajor)
            {
                Translate(point.Negative());
                RotateAlong(axis, radians);
                Translate(point);
            }
            else
            {
                Translate(point);
                RotateAlong(axis, radians);
                Translate(point.Negative());
            }
        }

        /// <summary>
        /// Rotates the affine transform around the given <paramref name="axis"/> at the given <paramref name="point"/>.
        /// </summary>
        /// <param name="point">Point at which to compute the rotation.</param>
        /// <param name="axis">The axis to rotate around. May be an addition of the basis vectors.</param>
        /// <param name="radians">Angle to rotate through.</param>
        /// <param name="order">The order to apply the transform in.</param>
        public void RotateAt(Vector point, Vector axis, Double radians, MatrixOperationOrder order)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translates the affine transform by the given amount 
        /// in each dimension.
        /// </summary>
        /// <param name="amount">Amount to translate by.</param>
        public void Translate(DoubleComponent amount)
        {
            Translate(amount, MatrixOperationOrder.Append);
        }

        /// <summary>
        /// Translates the affine transform by the given amount 
        /// in each dimension.
        /// </summary>
        /// <param name="amount">Amount to translate by.</param>
        /// <param name="order">The order to apply the transform in.</param>
        public void Translate(DoubleComponent amount, MatrixOperationOrder order)
        {
            Vector translateVector = new Vector(RowCount - 1);

            for (Int32 i = 0; i < translateVector.ComponentCount; i++)
            {
                translateVector[i] = amount;
            }

            AffineMatrix translateMatrix = new AffineMatrix(Format, RowCount);
            MatrixProcessor.Translate(translateMatrix, translateVector);

            Matrix result;

            if (order == MatrixOperationOrder.Append)
            {
                result = MatrixProcessor.Multiply(translateMatrix, this);
            }
            else
            {
                result = MatrixProcessor.Multiply(this, translateMatrix);
            }

            MatrixProcessor.SetMatrix(result, this);
        }

        /// <summary>
        /// Translates the affine transform by the given translation vector.
        /// </summary>
        /// <param name="translateVector">
        /// A vector whose components will translate the 
        /// transform in the corresponding dimension.
        /// </param>
        public void Translate(Vector translateVector)
        {
            Translate(translateVector, MatrixOperationOrder.Append);
        }

        /// <summary>
        /// Translates the affine transform by the given translation vector, in the order specified.
        /// </summary>
        /// <param name="translateVector">
        /// A vector whose components will translate the 
        /// transform in the corresponding dimension.
        /// </param>
        /// <param name="order">The order to apply the transform in.</param>
        public void Translate(Vector translateVector, MatrixOperationOrder order)
        {
            AffineMatrix translateMatrix = new AffineMatrix(Format, RowCount);
            MatrixProcessor.Translate(translateMatrix, translateVector);

            Matrix result;

            if (Format == MatrixFormat.RowMajor)
            {
                if (order == MatrixOperationOrder.Append)
                {
                    result = MatrixProcessor.Multiply(this, translateMatrix);
                }
                else
                {
                    result = MatrixProcessor.Multiply(translateMatrix, this);
                }
            }
            else
            {
                if (order == MatrixOperationOrder.Append)
                {
                    result = MatrixProcessor.Multiply(translateMatrix, this);
                }
                else
                {
                    result = MatrixProcessor.Multiply(this, translateMatrix);
                }
            }

            MatrixProcessor.SetMatrix(result, this);
        }

        #endregion

        #region ITransformMatrix<DoubleComponent,Vector,AffineMatrix> Members


        public AffineMatrix TransformMatrix(AffineMatrix input)
        {
            throw new NotImplementedException();
        }

        public new AffineMatrix TransformVector(Vector input)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMatrix<DoubleComponent,AffineMatrix> Members

        public new AffineMatrix Clone()
        {
            throw new NotImplementedException();
        }

        public new AffineMatrix GetMatrix(Int32 i0, Int32 i1, Int32 j0, Int32 j1)
        {
            throw new NotImplementedException();
        }

        AffineMatrix IMatrix<DoubleComponent, AffineMatrix>.GetMatrix(Int32[] rowIndexes, Int32 startColumn, Int32 endColumn)
        {
            throw new NotImplementedException();
        }

        public new AffineMatrix Transpose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<AffineMatrix> Members

        public Boolean Equals(AffineMatrix other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<AffineMatrix> Members

        public Int32 CompareTo(AffineMatrix other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<AffineMatrix> Members

        public new AffineMatrix Abs()
        {
            throw new NotImplementedException();
        }

        public new AffineMatrix Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INegatable<AffineMatrix> Members

        public new AffineMatrix Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<AffineMatrix> Members

        public AffineMatrix Subtract(AffineMatrix b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<AffineMatrix> Members

        public new AffineMatrix Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAddable<AffineMatrix> Members

        public AffineMatrix Add(AffineMatrix b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<AffineMatrix> Members

        public AffineMatrix Divide(AffineMatrix b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<AffineMatrix> Members

        public new AffineMatrix One
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IMultipliable<AffineMatrix> Members

        public AffineMatrix Multiply(AffineMatrix b)
        {
            return new AffineMatrix(MatrixProcessor.Multiply(this, b));
        }

        #endregion

        #region IBooleanComparable<AffineMatrix> Members

        public Boolean GreaterThan(AffineMatrix value)
        {
            throw new NotImplementedException();
        }

        public Boolean GreaterThanOrEqualTo(AffineMatrix value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThan(AffineMatrix value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThanOrEqualTo(AffineMatrix value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<AffineMatrix> Members

        AffineMatrix IExponential<AffineMatrix>.Exp()
        {
            throw new NotImplementedException();
        }

        AffineMatrix IExponential<AffineMatrix>.Log()
        {
            throw new NotImplementedException();
        }

        AffineMatrix IExponential<AffineMatrix>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        AffineMatrix IExponential<AffineMatrix>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        AffineMatrix IExponential<AffineMatrix>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAffineTransformMatrix<DoubleComponent> Members

        IAffineTransformMatrix<DoubleComponent> IAffineTransformMatrix<DoubleComponent>.Inverse
        {
            get { throw new NotImplementedException(); }
        }

        void IAffineTransformMatrix<DoubleComponent>.RotateAt(IVector<DoubleComponent> point, IVector<DoubleComponent> axis, double radians, MatrixOperationOrder order)
        {
            throw new NotImplementedException();
        }

        void IAffineTransformMatrix<DoubleComponent>.RotateAt(IVector<DoubleComponent> point, IVector<DoubleComponent> axis, double radians)
        {
            throw new NotImplementedException();
        }

        void IAffineTransformMatrix<DoubleComponent>.Translate(IVector<DoubleComponent> translateVector, MatrixOperationOrder order)
        {
            throw new NotImplementedException();
        }

        public void Translate(IVector<DoubleComponent> translateVector)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMatrix<DoubleComponent> Members

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Clone()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.GetMatrix(Int32[] rowIndexes, Int32 startColumn, Int32 endColumn)
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Inverse
        {
            get { throw new NotImplementedException(); }
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Transpose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<IMatrix<DoubleComponent>> Members

        Boolean IEquatable<IMatrix<DoubleComponent>>.Equals(IMatrix<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<IMatrix<DoubleComponent>> Members

        Int32 IComparable<IMatrix<DoubleComponent>>.CompareTo(IMatrix<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IComputable<IMatrix<DoubleComponent>>.Abs()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IComputable<IMatrix<DoubleComponent>>.Set(double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INegatable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> INegatable<IMatrix<DoubleComponent>>.Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> ISubtractable<IMatrix<DoubleComponent>>.Subtract(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IHasZero<IMatrix<DoubleComponent>>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAddable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IAddable<IMatrix<DoubleComponent>>.Add(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IDivisible<IMatrix<DoubleComponent>>.Divide(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IHasOne<IMatrix<DoubleComponent>>.One
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IMultipliable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IMultipliable<IMatrix<DoubleComponent>>.Multiply(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IMatrix<DoubleComponent>> Members

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.GreaterThan(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.GreaterThanOrEqualTo(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.LessThan(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.LessThanOrEqualTo(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Exp()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Log()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Log(double newBase)
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Power(double exponent)
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Helper Methods

        private static void checkSquare(Int32 rowCount, Int32 columnCount)
        {
            if (rowCount != columnCount)
            {
                throw new ArgumentException("An affine transform matrix must be square, " +
                                            "so rowCount must equal columnCount.");
            }
        }

        //private static void homogeonizeVector(ref DoubleComponent[] vector)
        //{
        //    Array.Resize(ref vector, vector.Length + 1);
        //    vector[vector.Length - 1] = _one;
        //}

        //private IEnumerable<Vector> homogeonizeVectors(IEnumerable<Vector> vectors)
        //{
        //    if(vectors == null)
        //    {
        //        yield break;
        //    }

        //    foreach (Vector vector in vectors)
        //    {
        //        if (vector.ComponentCount != ColumnCount - 1)
        //        {
        //            throw new InvalidOperationException(
        //                String.Format("Vector to transform must have the same number of components as transform rank. Expected: {0}, encountered: {1}", ColumnCount, vector.ComponentCount));
        //        }

        //        DoubleComponent[] components = vector.Components;
        //        homogeonizeVector(ref components);
        //        yield return new Vector<T>(vector.Format, components);
        //    }
        //}

        #endregion
    }
}