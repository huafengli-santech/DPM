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
#2
GLOBAL INT Axis0=0,Axis1=1,Axis2=2
PE_0_Threshold=0.1;PE_1_Threshold=0.1;PE_2_Threshold=0.1;
measure_continuously = 1
Sample_set_size = 20
!Start Measurement
PE_0.Stop();PE_1.Stop();PE_2.Stop();
Motion_status_0.SelectAxis(0);Motion_status_1.SelectAxis(1);Motion_status_2.SelectAxis(2);
Motion_status_0.MonitorOn();Motion_status_1.MonitorOn();Motion_status_2.MonitorOn();
!Measurement activation
PE_0.MeasureProcess(PE(Axis0),Motion_status_0.during_cv,Sample_set_size, measure_continuously);
PE_1.MeasureProcess(PE(Axis1),Motion_status_1.during_cv,Sample_set_size, measure_continuously);
PE_2.MeasureProcess(PE(Axis2),Motion_status_2.during_cv,Sample_set_size, measure_continuously);
STOP





