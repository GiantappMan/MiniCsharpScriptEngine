# 初衷：可以在Converter中运用一些简单的c#语法

		Debug.Assert(Test("True||False").ToString() == "True");
		Debug.Assert(Test("True?111:222").ToString() == "111");

		Debug.Assert(Test("(bool)True||False").ToString() == true.ToString());
		Debug.Assert(Test("(int)True?111:222").ToString() == 111.ToString());
		Debug.Assert(Test("(bool)1>2").ToString() == false.ToString());
		Debug.Assert(Test("(bool)1<2").ToString() == true.ToString());
		Debug.Assert(Test("(bool)False||False||1<2").ToString() == true.ToString());
		Debug.Assert(Test("(bool)True||False||1<2").ToString() == true.ToString());
		Debug.Assert(Test("(bool)False||False||1>2").ToString() == false.ToString());

		Debug.Assert(Test("(bool)False||False||(2>1&&1<2)").ToString() == true.ToString());
		Debug.Assert(Test("(bool)1<2&&True&&(True==False||1<2)").ToString() == true.ToString());
		Debug.Assert(Test("(bool)1<2&&True&&((True==False)||1<2)").ToString() == true.ToString());
		Debug.Assert(Test("(bool)1<2&&True&&((True==False)||1>2)").ToString() == false.ToString());
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
	   
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
