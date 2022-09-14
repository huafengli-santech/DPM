#0
GLOBAL INT Axis0=0,Axis2=2
PE_0_Threshold=0.1;PE_2_Threshold=0.2;
measure_continuously = 1
Sample_set_size = 20
!Start Measurement
PE_0.Stop();PE_2.Stop();
Motion_status_0.SelectAxis(0);Motion_status_2.SelectAxis(2);
Motion_status_0.MonitorOn();Motion_status_2.MonitorOn();
!Measurement activation
PE_0.MeasureProcess(PE(Axis0),Motion_status_0.during_cv,Sample_set_size, measure_continuously);
PE_2.MeasureProcess(PE(Axis2),Motion_status_2.during_cv,Sample_set_size, measure_continuously);

STOP
#1
void GetPara(real inTimeArr(),real ref outParaArr(),real aJerk,real delay,real resolution)
GLOBAL REAL Tx(8),Para(4),pJerk,delay,resolution
resolution = 1
delay = 1
pJerk = 10
Tx(0) = 0.10
Tx(1) = 0.20
Tx(2) = 0.50
Tx(3) = 0.60
Tx(4) = 1
Tx(5) = 1.10
Tx(6) = 1.40
Tx(7) = 1.50
START 2,1
GetPara(Tx,Para,pJerk,delay,resolution)
STOP
void GetPara(real inTimeArr(),real ref outParaArr(),real aJerk,real delay,real resolution)
{
	REAL T,Xx(8),Vx(7),Ax
	REAL Xpos,Vel,Acc,Jerk
	T=0
	WHILE T<inTimeArr(7)+delay
		BLOCK
			IF (T<inTimeArr(0))
				Xpos = 0
				Vel  = 0
				Acc  = 0
				Jerk = 0
			ELSEIF (inTimeArr(0)<T & T<inTimeArr(1))
				Jerk  = aJerk
				Acc   = aJerk*(T-inTimeArr(0))
				Vel   = 0.5*aJerk*Pow((T-inTimeArr(0)),2)
				Xpos  = 0.1667*aJerk*Pow((T-inTimeArr(0)),3)
				Vx(1) = Vel
				Xx(1) = Xpos	
				Ax    = Acc	
			ELSEIF (inTimeArr(1)<T & T<inTimeArr(2))
				Jerk = 0
				Acc = Ax
				Vel  = Vx(1)+Acc*(T-inTimeArr(1))
				Xpos  = Xx(1)+Vx(1)*(T-inTimeArr(1))+0.5*Ax*Pow((T-inTimeArr(1)),2)
				Vx(2) = Vel
				Xx(2) = Xpos
			ELSEIF (inTimeArr(2)<T & T<inTimeArr(3))
				Jerk = -aJerk
				Acc = Ax-aJerk*(T-inTimeArr(2))
				Vel  = Vx(2)+Ax*(T-inTimeArr(2))-0.5*aJerk*Pow((T-inTimeArr(2)),2)
				Xpos  = Xx(2)+Vx(2)*(T-inTimeArr(2))+0.5*Ax*Pow((T-inTimeArr(2)),2)-0.1667*aJerk*Pow((T-inTimeArr(2)),3)
				Vx(3) = Vel
				Xx(3) = Xpos
			ELSEIF (inTimeArr(3)<T & T<inTimeArr(4))
				Jerk = 0
				Acc = 0
				Vel  = Vx(3)
				Xpos  = Xx(3)+Vx(3)*(T-inTimeArr(3))
				Vx(4) = Vel
				Xx(4) = Xpos
			ELSEIF (inTimeArr(4)<T & T<inTimeArr(5))
				Jerk = -aJerk
				Acc = -aJerk*(T-inTimeArr(4))
				Vel  = Vx(4)-0.5*aJerk*Pow((T-inTimeArr(4)),2)
				Xpos  = Xx(4)+Vx(4)*(T-inTimeArr(4))-0.1667*aJerk*Pow((T-inTimeArr(4)),3)
				Vx(5) = Vel
				Xx(5) = Xpos
				
			ELSEIF (inTimeArr(5)<T & T<inTimeArr(6))
				Jerk = 0
				Acc = -Ax
				Vel  = Vx(5)-Ax*(T-inTimeArr(5))
				Xpos  = Xx(5)+Vx(3)*(T-inTimeArr(5))-0.5*Ax*Pow((T-inTimeArr(5)),2)
				Vx(6) = Vel
				Xx(6) = Xpos
			ELSEIF (inTimeArr(6)<T & T<inTimeArr(7))
				Jerk = aJerk
				Acc = -Ax+aJerk*(T-inTimeArr(6))
				Vel  = Vx(6)-Ax*(T-inTimeArr(6))+0.5*aJerk*Pow((T-inTimeArr(6)),2)
				Xpos  = Xx(6)+Vx(6)*(T-inTimeArr(6))-0.5*Ax*Pow((T-inTimeArr(6)),2)+0.1667*aJerk*Pow(T-inTimeArr(6),3)
				Xx(7) = Xpos
			ELSEIF (T>inTimeArr(7))
				Jerk = 0
				Acc = 0
				Vel  = 0
				Xpos  = Xx(7)
			END
	!		DISP Xpos
			T = T + resolution*0.001
			outParaArr(0)=Xpos
			outParaArr(1)=Vel
			outParaArr(2)=Acc
			outParaArr(3)=Jerk
		END
	END
RET}
#2
GLOBAL REAL Para(4)
DISP "XPOS\tVEL\tACC\tJERK"
WHILE PST(1).#RUN
	DISP Para(0),Para(1),Para(2),Para(3)
END
STOP





