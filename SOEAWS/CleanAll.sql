use SOE_DATABASE


delete From grades
where INSCRIPTION_ID
IN
(
SELECT ID from INSCRIPTIONS
where group_id=14
)

delete from INSCRIPTIONS
where group_id=14

delete from CLASS_TIMES
where TEACHER_SUBJECT_ID in
(
select id from TEACHER_SUBJECTS where 
SUBJECT_ID
in
(
select id from subjects where color_id=1
)
)

delete from TEACHER_SUBJECTS where 
SUBJECT_ID
in
(
select id from subjects where color_id=1
)

delete from subjects where color_id=1