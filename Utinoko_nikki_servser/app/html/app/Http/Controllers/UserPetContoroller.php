<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class UserPetContoroller extends Controller
{
    // 育成中ペット取得
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');
        $elderUserId = $request->query('elder_user_id');

        $query = DB::table('user_pets')
            ->select(
                'id',
                'elder_user_id',
                'animals_id',
                'nickname',
                'created_at',
                'updated_at'
            );

        if ($id !== null && $id !== '') {
            $item = $query->where('id', $id)->first();

            return response()->json($item);
        }

        if ($elderUserId !== null && $elderUserId !== '') {
            $query->where('elder_user_id', $elderUserId);
        }

        return response()->json($query->get());
    }

    // 育成ペット登録
    public function store(Request $request): JsonResponse
    {
        $validated = $request->validate([
            'elder_user_id' => 'required|integer|exists:elder_users,id',
            'animals_id' => 'required|integer|exists:user_pets,id',
        ]);

        $id = DB::table('user_pets')->insertGetId([
            'elder_user_id' => $validated['elder_user_id'],
            'animals_id' => $validated['animals_id'],
            'nickname' => $request['nickname'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        $item = DB::table('user_pets')
            ->where('id', $id)
            ->first();

        return response()->json($item, 201);
    }

    //育成ペット編集
    public function update(Request $request, int $id): JsonResponse
    {
        // 対象のペットが存在するか確認
        $pet = DB::table('user_pets')
            ->where('id', $id)
            ->first();

        if (!$pet) {
            return response()->json([
                'message' => '指定されたペットが存在しません。'
            ], 404);
        }

        // バリデーション
        $validated = $request->validate([
            'nickname' => 'required|string|max:255',
        ]);

        try {
            // ペット情報を更新
            DB::table('user_pets')
                ->where('id', $id)
                ->update([
                    'nickname' => $validated['nickname'],
                    'updated_at' => now(),
                ]);

            // 更新後のデータを取得
            $pet = DB::table('user_pets')
                ->where('id', $id)
                ->first();

            return response()->json($pet, 200);

        } catch (\Illuminate\Database\QueryException $e) {
            return response()->json([
                'message' => 'ペットの更新中にデータベースエラーが発生しました。'
            ], 500);
        }
    }

    //育成ペット削除
    public function destroy(int $id): JsonResponse
    {
        // 対象のペットが存在するか確認
        $pet = DB::table('user_pets')
            ->where('id', $id)
            ->first();

        if (!$pet) {
            return response()->json([
                'message' => '指定されたペットが存在しません。'
            ], 404);
        }

        try {
            // ペットを削除
            DB::table('user_pets')
                ->where('id', $id)
                ->delete();

            return response()->json([
                'message' => 'ペットを削除しました。'
            ], 200);

        } catch (\Illuminate\Database\QueryException $e) {

            // 外部キー制約違反
            if (($e->errorInfo[1] ?? null) === 1451) {
                return response()->json([
                    'message' => 'このペットは他のデータから参照されているため削除できません。'
                ], 409);
            }

            // その他のDBエラー
            return response()->json([
                'message' => 'ペットの削除中にデータベースエラーが発生しました。'
            ], 500);
        }
    }
}
