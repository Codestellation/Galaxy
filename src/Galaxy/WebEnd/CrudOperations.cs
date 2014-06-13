using System;

namespace Codestellation.Galaxy.WebEnd
{
    [Flags]
    public enum CrudOperations
    {
        GetList = 1 << 0,
        GetDetails = 1 << 1,

        GetCreate = 1 << 2,
        PostCreate = 1 << 3,

        GetEdit = 1 << 4,
        PostEdit = 1 << 5,

        GetDelete = 1 << 6,
        PostDelete = 1 << 7,
    }
}