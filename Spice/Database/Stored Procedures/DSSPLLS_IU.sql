 CREATE PROCEDURE IPTSQUADSY/DSSPLLS_IU                             
 (IN P_CLASS DEC (4, 0),                                       
  IN P_VENDOR DEC (5, 0),                                      
  IN P_STYLE DEC (4, 0),                                       
  IN P_COLOUR DEC (3, 0),                                      
  IN P_LANG CHAR (12),                                         
  IN P_DESC  GRAPHIC (30) CCSID 13488 )                        
 LANGUAGE SQL                                                  
 MODIFIES SQL DATA                                             
 BEGIN                                                         
  IF EXISTS ( SELECT * FROM DSSPISD WHERE ISDCLS=P_CLASS AND   
  ISDVEN=P_VENDOR AND ISDSTY=P_STYLE AND ISDCLR=P_COLOUR AND   
  ISDLAN=P_LANG ) THEN                                         
                                                               
  UPDATE DSSPISD SET ISDDES = P_DESC  WHERE ISDCLS=P_CLASS AND 
  ISDVEN=P_VENDOR AND ISDSTY=P_STYLE AND ISDCLR=P_COLOUR AND   
  ISDLAN=P_LANG;                                                   
                                                                  
 ELSE                                                             
   INSERT INTO DSSPISD                                            
  (ISDCLS,ISDVEN,ISDCLR,ISDSTY,ISDLAN,ISDDES)                     
   VALUES (P_CLASS,P_VENDOR,P_COLOUR,P_STYLE,P_LANG,P_DESC);      
 END IF;                                                          
                                                                  
END  




