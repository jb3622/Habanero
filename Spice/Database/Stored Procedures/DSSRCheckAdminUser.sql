 CREATE PROCEDURE IPTSQUADSY/DSSRAdmUsr                             
 (IN P_USERID VARCHAR(10))                        
 LANGUAGE SQL                                                  
 MODIFIES SQL DATA                                             
 BEGIN                                                         
	SELECT * FROM DSSRADM
	WHERE USERID = P_USERID;
  
END  

