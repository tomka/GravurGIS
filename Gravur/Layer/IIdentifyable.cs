namespace GravurGIS.Layers
{
    interface IIdentifyable
    {
        int[] identify(double x, double y);
        
        void clearSelection();

        bool IsIndexed
        {
            get;
        }
        bool IsWellDefined
        {
            get;
            set;
        }
    }
}
