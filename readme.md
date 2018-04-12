# 初衷：可以在Converter中运用一些简单的c#语法

		Debug.Assert(Eval("True||False").ToString() == "True");
		Debug.Assert(Eval("True?111:222").ToString() == "111");

		Debug.Assert(Eval("(bool)True||False").ToString() == true.ToString());
		Debug.Assert(Eval("(int)True?111:222").ToString() == 111.ToString());
		Debug.Assert(Eval("(bool)1>2").ToString() == false.ToString());
		Debug.Assert(Eval("(bool)1<2").ToString() == true.ToString());
		Debug.Assert(Eval("(bool)False||False||1<2").ToString() == true.ToString());
		Debug.Assert(Eval("(bool)True||False||1<2").ToString() == true.ToString());
		Debug.Assert(Eval("(bool)False||False||1>2").ToString() == false.ToString());

		Debug.Assert(Eval("(bool)False||False||(2>1&&1<2)").ToString() == true.ToString());
		Debug.Assert(Eval("(bool)1<2&&True&&(True==False||1<2)").ToString() == true.ToString());
		Debug.Assert(Eval("(bool)1<2&&True&&((True==False)||1<2)").ToString() == true.ToString());
		Debug.Assert(Eval("(bool)1<2&&True&&((True==False)||1>2)").ToString() == false.ToString());

		Debug.Assert(Eval("(bool)!True").ToString() == false.ToString());
		Debug.Assert(Eval("(bool)!False").ToString() == true.ToString());
		Debug.Assert(Eval("(Visibility)!False?Visible:Collapsed").ToString() == Visibility.Visible.ToString());
		Debug.Assert(Eval("(Visibility)False?Visible:Collapsed").ToString() == Visibility.Collapsed.ToString());

		Debug.Assert(Eval("(int)1+1").ToString() == 2.ToString());
		Debug.Assert(Eval("(int)2-1").ToString() == 1.ToString());
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
	   
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
       
       

       
       
       
       
       
       
       

       
       
       
       
