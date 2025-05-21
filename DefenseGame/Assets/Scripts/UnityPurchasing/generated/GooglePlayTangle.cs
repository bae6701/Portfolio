// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("MCk5VHuxMfdF2+2BSjGbKT43jqjUV1lWZtRXXFTUV1dW4zFfFJkjFvcLPo3CLCBNMuvCIW2Ds9R6Aw4PUP5LNmaRmZUcrOVbDLbd+1NUpQBuFGSBr2lG+MLO2289kHIJNi+IrB3sgNwOn3HuNpRBXsnkRbdfzxVyh+jAReYwBwZwb91olgrt9XZrhWnRBaIo8nKS47rg80/3uyO+n3GJ4bt1bHMVUST+fvBZxuG9PHB6z02MZtRXdGZbUF980B7QoVtXV1dTVlV1L+dyg8++J8kVCh3H+nWl7zbezYy/VCgYF7T0kwnahVXdsCO5RBlLKyKM2TEdvUjnEFsdp7bYUtm20cErdylyLq8FtaVK9AUZKw/wdginsMSaUjJsRmpPt1RVV1ZX");
        private static int[] order = new int[] { 7,1,13,11,8,12,13,8,10,10,10,11,13,13,14 };
        private static int key = 86;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
