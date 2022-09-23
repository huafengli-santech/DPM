#0
GLOBAL REAL Xpos,Vel,Acc,Jerk
GLOBAL REAL aJerk
GLOBAL REAL InTime(1),T,Tx(8)
GLOBAL REAL Xx(8),Vx(7),Ax
Tx(0) = 0.10
Tx(1) = 0.20
Tx(2) = 0.50
Tx(3) = 0.60
Tx(4) = 1
Tx(5) = 1.10
Tx(6) = 1.40
Tx(7) = 1.50
aJerk = 10
WAIT 500
T=0
WHILE T<Tx(7)+0.29
	BLOCK
		IF (T<Tx(0))
			Xpos = 0
			Vel  = 0
			Acc  = 0
			Jerk = 0
		ELSEIF (Tx(0)<T & T<Tx(1))
			Jerk  = aJerk
			Acc   = aJerk*(T-Tx(0))
			Vel   = 0.5*aJerk*Pow((T-Tx(0)),2)
			Xpos  = 0.1667*aJerk*Pow((T-Tx(0)),3)
			Vx(1) = Vel
			Xx(1) = Xpos	
			Ax    = Acc	
		ELSEIF (Tx(1)<T & T<Tx(2))
			Jerk = 0
			Acc = Ax
			Vel  = Vx(1)+Acc*(T-Tx(1))
			Xpos  = Xx(1)+Vx(1)*(T-Tx(1))+0.5*Ax*Pow((T-Tx(1)),2)
			Vx(2) = Vel
			Xx(2) = Xpos
		ELSEIF (Tx(2)<T & T<Tx(3))
			Jerk = -aJerk
			Acc = Ax-aJerk*(T-Tx(2))
			Vel  = Vx(2)+Ax*(T-Tx(2))-0.5*aJerk*Pow((T-Tx(2)),2)
			Xpos  = Xx(2)+Vx(2)*(T-Tx(2))+0.5*Ax*Pow((T-Tx(2)),2)-0.1667*aJerk*Pow((T-Tx(2)),3)
			Vx(3) = Vel
			Xx(3) = Xpos
		ELSEIF (Tx(3)<T & T<Tx(4))
			Jerk = 0
			Acc = 0
			Vel  = Vx(3)
			Xpos  = Xx(3)+Vx(3)*(T-Tx(3))
			Vx(4) = Vel
			Xx(4) = Xpos
		ELSEIF (Tx(4)<T & T<Tx(5))
			Jerk = -aJerk
			Acc = -aJerk*(T-Tx(4))
			Vel  = Vx(4)-0.5*aJerk*Pow((T-Tx(4)),2)
			Xpos  = Xx(4)+Vx(4)*(T-Tx(4))-0.1667*aJerk*Pow((T-Tx(4)),3)
			Vx(5) = Vel
			Xx(5) = Xpos
			
		ELSEIF (Tx(5)<T & T<Tx(6))
			Jerk = 0
			Acc = -Ax
			Vel  = Vx(5)-Ax*(T-Tx(5))
			Xpos  = Xx(5)+Vx(3)*(T-Tx(5))-0.5*Ax*Pow((T-Tx(5)),2)
			Vx(6) = Vel
			Xx(6) = Xpos
			DISP "5-6",T,Xpos
		ELSEIF (Tx(6)<T & T<Tx(7))
			Jerk = aJerk
			Acc = -Ax+aJerk*(T-Tx(6))
			Vel  = Vx(6)-Ax*(T-Tx(6))+0.5*aJerk*Pow((T-Tx(6)),2)
			Xpos  = Xx(6)+Vx(6)*(T-Tx(6))-0.5*Ax*Pow((T-Tx(6)),2)+0.1667*aJerk*Pow(T-Tx(6),3)
			Xx(7) = Xpos
		ELSEIF (T>Tx(7))
			Jerk = 0
			Acc = 0
			Vel  = 0
			Xpos  = Xx(7)
		END
!		DISP Xpos
		T = T +0.001
	END
END
STOP

#A
!axisdef X=0,Y=1,Z=2,T=3,A=4,B=5,C=6,D=7
!axisdef x=0,y=1,z=2,t=3,a=4,b=5,c=6,d=7
global int I(100),I0,I1,I2,I3,I4,I5,I6,I7,I8,I9,I90,I91,I92,I93,I94,I95,I96,I97,I98,I99
global real V(100),V0,V1,V2,V3,V4,V5,V6,V7,V8,V9,V90,V91,V92,V93,V94,V95,V96,V97,V98,V99

!DPM Test
GLOBAL STATIC DPM_Measurement PE_0
GLOBAL STATIC DPM_Motion_Status Motion_status_0
GLOBAL STATIC REAL PE_0_Threshold
GLOBAL  INT Sample_set_size
GLOBAL STATIC INT measure_continuously



